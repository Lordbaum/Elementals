using System;
using Elements;
using Generall;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace MovmentUndSo
{
    public class RigidbodyMovment : NetworkBehaviour

    {
        public Rigidbody rigid;
        public Transform orientation;
        public float normalSpeed = 200f;
        public float sprintMultiplier = 2f;
        public float dashMultiplier = 3f;
        public float gravity = -20f;
        public float jumpHight = 3f;
        public float airDrag, groundDrag;
        public Transform groundCheck;
        public float groundDistance;
        public LayerMask groundMask;
        public bool isGrounded;
        public bool isFreezed;
        public bool usingGravity = true;
        public ParticleSystem airJump;
        public float climbingSpeed = 3f;

        //wall running
        public LayerMask whatIsWall;
        public float wallrunForce, maxWallrunSpeed, maxWallrunTime;
        public bool isWallLeft, isWallRight;
        public bool isWallrunning;
        public float wallRunCameraTilt, maxCameratilt;
        public ParticleSystem wallRunParticles;
        public float maxDashTilt;
        private bool dashing;

        private float dashTilt;
        private Elementselect elementselect;
        private int extraJumps;
        private Vector3 move;
        private NetworkRigidbody networkRigidbody;
        private bool noMove;
        private Vector3 rigidVelocity;
        private ServerPlayerControll spc;
        private float speed;
        private bool sprinting;
        private Vector3 startScale;


        void Start()
        {
            elementselect = GetComponent<Elementselect>();
            extraJumps = elementselect.air;
            startScale = transform.localScale;
            spc = GetComponent<ServerPlayerControll>();
        }

        // Update is called once per frame

        public void Update()
        {
            //checks if this is the player
            if (!GetComponent<NetworkObject>().IsLocalPlayer) return;
            MovmentServerRpc();
        }


        public void FixedUpdate()
        {
            //checks if this is the player
            if (!GetComponent<NetworkObject>().IsLocalPlayer) return;
            FixedMovmentServerRpc();
        }

        [ServerRpc]
        private void MovmentServerRpc()
        {
            //ServerRpc can used by Clients
            ServerRpcAttribute serverRpcAttribute = new ServerRpcAttribute {RequireOwnership = false};
            if (!GetComponent<NetworkObject>().IsOwner) return;
            //sorgt für passende Rotation
            RotationUpdater();
            //wenn Erde ausgrüßtet ist und es nicht gefreezed worden ist kann man Wallrunnen
            if (elementselect.IsElementActive("Earth") && !isFreezed)
            {
                Wallrun();
            }
            else
                //wenn nicht werden die Wall variablen auf false geschaltet und das Wallrunnen gestiopt
            {
                isWallLeft = false;
                isWallRight = false;
                StopWallrun();
            }
//Wenn man Leertaste frückt und nicht gefreezed ist und man auf dem Boden ist, noch extrasprünge hat oder an der Wand bist
//und zurseite drückst, Springst du

            if (Input.GetButtonDown("Jump") && !isFreezed &&
                (isGrounded || elementselect.IsElementActive("Air") && extraJumps > 0)
                || isWallrunning && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
            {
                Jump();
            }

//Aufdem Boden
            if (isGrounded)
            {
                //wenn man nicht Dashed ist der Drag normal
                if (!dashing)
                {
                    rigid.drag = groundDrag;
                }

                //Geschwindigkeit und zurücksetzen der extra sprünge
                speed = normalSpeed;
                extraJumps = elementselect.GetElementLevel("Air");

                // normales sprinten
                if (Input.GetKey(KeyCode.LeftShift) && !isFreezed)
                {
                    speed = sprintMultiplier * normalSpeed;
                    sprinting = true;
                }
                else sprinting = false;


//Wenn Strg gedrückt wird
                if (Input.GetKeyDown(KeyCode.LeftControl) && !isFreezed)
                {
                    //gibt ihm ein Boost und startet den Dash
                    speed = dashMultiplier * normalSpeed;
                    StartDash();
                }
                //wenn man die Taste loslässt oder die durschnits geschwindigkeit kleiner als drei ist wird das Dashen gestoppt
                else if (Input.GetKeyUp(KeyCode.LeftControl) || isFreezed || Mathf.Abs(rigidVelocity.x) +
                         Mathf.Abs(rigidVelocity.y) + Mathf.Abs(rigidVelocity.z) / 3 < 3)
                {
                    StopDash();
                }
                else if (dashing)
                {
                    Dash();
                }

                if (Mathf.Abs((rigidVelocity.x + rigidVelocity.y + rigidVelocity.z) / 3) < 0.001f)
                {
                    //rigid.velocity = Vector3.zero;
                }
            }
            //Wenn man in der Luft ist
            else
            {
                //verändert den Drag in der Luft
                rigid.drag = airDrag;
                //wenn man Dashed wird das Dashen gestoppt
                if (dashing)
                {
                    StopDash();
                }
            }

//ersetzt die Schwerkrafft für den Spiler aber nur wenn sie genutzt wird und man nicht schneller ist als -100 in y richtung
            if (rigidVelocity.y > -100 && usingGravity)
            {
                rigid.AddForce(-orientation.up * (gravity * Time.deltaTime), ForceMode.Impulse);
            }
        }

        [ServerRpc]
        private void FixedMovmentServerRpc()
        {
            //ServerRpc can used by Clients
            ServerRpcAttribute serverRpcAttribute = new ServerRpcAttribute {RequireOwnership = false};
            if (!GetComponent<NetworkObject>().IsOwner) return;
            rigidVelocity = rigid.velocity;
            //Checked ob man auf dem Boden ist
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


//wenn man auf dem Boden ist
            if (isGrounded)
            {
                if (isFreezed)
                {
                    speed = 0;
                }
                //nach einem freez und die constraints auf None steht werden sie zurückgestzt und der Spieler aufgerichtet
                else if (rigid.constraints == RigidbodyConstraints.None)
                {
                    rigid.constraints = RigidbodyConstraints.FreezeRotation;
                    rigid.rotation.eulerAngles.Set(0, rigid.rotation.eulerAngles.y, 0);
                }

//Bewegung
                rigid.AddForce(Move() * speed);
            }
            else
            {
                //in der luft bewegt man sich langsamer
                rigid.AddForce(Move() * speed / 2.5f);
            }

// wenn man nicht sprintet oder Dashed wird eine Maximal geschwindigkeit festgelegt
            if (!sprinting && !dashing)
            {
                rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x, -30, 30), rigid.velocity.y,
                    Mathf.Clamp(rigid.velocity.z, -30, 30));
            }
            else if (!dashing)
                //eine Maximal geschwindigkeit fürs Sprinten
            {
                rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x, -50, 50), rigid.velocity.y,
                    Mathf.Clamp(rigid.velocity.z, -50, 50));
            }

//Code für Tolpatschige Entwickler
            if (Input.GetKeyDown(KeyCode.P))
            {
                transform.position = new Vector3(3, 4, 34);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        /* public void OnCollisionStay(Collision other)
         {
             //wen man mit einer Pflanze kollidert und man Pflanze ausgerüßtet hat und Space drückt
             if (elementselect.IsElementActive("Plant") && other.gameObject.TryGetComponent(out Objectdata data)
                                                        && data.HasElement("Plant") && Input.GetKey(KeyCode.Space))
             {
                 //kletter man
                 rigid.AddForce(transform.up * (climbingSpeed * elementselect.plant));
             }
         }*/

        private Vector3 Move()
        {
            //setzt das Movment jenachdem was man drückt
            move = Vector3.zero;
            if (!isFreezed || !noMove)
            {
                if (Input.GetKey(KeyCode.W)) move += transform.forward;
                if (Input.GetKey(KeyCode.A) && !isWallrunning) move += transform.TransformVector(Vector3.left);
                if (Input.GetKey(KeyCode.S) && !isWallrunning) move += transform.TransformVector(Vector3.back);
                if (Input.GetKey(KeyCode.D) && !isWallrunning) move += transform.right;
            }

            return move;
        }

        private void Jump()
        {
            Vector3 airForward = Vector3.zero;
            //in der Luft wird ein extra Sprung abgezogen die x und y bewegungen negiert die entsprechenden Partikel abgespielt und ein sprung
            //nach vorne gemacht
            if (!isGrounded && !isWallrunning)
            {
                airJump.Play();
                extraJumps--;
                rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
                airForward = transform.forward * speed / 10;
            }
            //wenn man Wallrunned kann man zur seite Springen
            else if (isWallrunning)
            {
                if (isWallLeft && Input.GetKey(KeyCode.D))
                {
                    airForward = transform.TransformVector(Vector3.right) * speed / 10;
                }
                else if (isWallRight && Input.GetKey(KeyCode.S))
                {
                    airForward = transform.TransformVector(Vector3.left) * speed / 10;
                }
            }


            rigid.AddForce(orientation.up * Mathf.Sqrt(jumpHight * 2 * gravity) + airForward, ForceMode.Impulse);
//berechnet die geschwindigkeit des Sprungs
        }


        private void Wallrun()
        {
            //Checkt ob man Wallruned
            CheckForWallrun();
            //Schaut nach dem Input
            WallrunInput();
            if (rigid.velocity.sqrMagnitude <= maxWallrunSpeed * elementselect.earth && isWallrunning)
            {
                //Moved den Speiler nach vorne
                rigid.AddForce(orientation.forward * wallrunForce * Time.deltaTime, ForceMode.Acceleration);
                //drücked den Spieler zur Wand
                if (isWallLeft)
                {
                    rigid.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
                }
                else
                {
                    rigid.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
                }
            }
        }

        private void WallrunInput()
        {
            //Schaut ob eine Wand neben dem Spieler ist und ob man die Taste zu ihr gedrückt hat, wenn ja wird der Wallrun gestarted
            if (Input.GetKeyDown(KeyCode.A) && isWallLeft) StartWallrun();
            if (Input.GetKeyDown(KeyCode.D) && isWallRight) StartWallrun();
        }

        private void StartWallrun()
        {
            //sagt das er jetzt wall runnend und die Gravitation wird ncihtmehr genutzt
            isWallrunning = true;
            usingGravity = false;
//spielt entsprechende ab
            wallRunParticles.Play();
        }

        private void StopWallrun()
        {
            //setzt die variablen wieder zum default
            isWallrunning = false;
            usingGravity = true;
            wallRunParticles.Stop();
        }

        private void CheckForWallrun()
        {
            //schaut ob Rechts und links eine Steinwand ist mitteles Raycasts
            isWallLeft = Physics.Raycast(orientation.position, orientation.TransformVector(Vector3.left), out var hitL,
                             1f, whatIsWall)
                         && hitL.transform.TryGetComponent(out Objectdata dataL) && dataL.HasElement("Earth");
            isWallRight = Physics.Raycast(orientation.position, orientation.TransformVector(Vector3.right),
                              out var hitR, 1f, whatIsWall)
                          && hitR.transform.TryGetComponent(out Objectdata dataR) && dataR.HasElement("Earth");
            //wenn dort keine ist hört der spieler auf mit Wallrunning
            if (!isWallLeft && !isWallRight)
            {
                StopWallrun();
            }

            //verändert für die Seiten entsprechend die Particel
            if (isWallLeft)
            {
                wallRunParticles.shape.rotation.Set(0, 90, 0);
                wallRunParticles.GetComponent<Renderer>().materials[0] = hitL.transform.GetComponent<Material>();
            }
            else if (isWallRight)
            {
                wallRunParticles.shape.rotation.Set(0, -90, 0);
                wallRunParticles.GetComponent<Renderer>().materials[0] = hitR.transform.GetComponent<Material>();
            }
        }

        private void RotationUpdater()
        {
            //tilts the body while wallrunning
            if (Math.Abs(wallRunCameraTilt) < maxCameratilt && isWallrunning && isWallLeft)
            {
                wallRunCameraTilt -= Time.deltaTime * maxCameratilt * 4;
            }

            if (Math.Abs(wallRunCameraTilt) < maxCameratilt && isWallrunning && isWallRight)
            {
                wallRunCameraTilt += Time.deltaTime * maxCameratilt * 4;
            }

//tillts body back again on the z Acces
            if (wallRunCameraTilt < 0 && !isWallLeft && !isWallRight)
            {
                wallRunCameraTilt += Time.deltaTime * maxCameratilt * 4;
            }

            if (wallRunCameraTilt > 0 && !isWallLeft && !isWallRight)
            {
                wallRunCameraTilt -= Time.deltaTime * maxCameratilt * 4;
            }

            //kümmert sich um die Rotation beim Dashen
            if (dashing && dashTilt > maxDashTilt)
            {
                dashTilt -= Time.deltaTime * maxDashTilt * -5;
            }
            //setzt sie wieder auf normal zurück
            else if (!dashing && dashTilt < 0)
            {
                dashTilt += Time.deltaTime * maxDashTilt * -5;
            }

            //perform rotation
            transform.rotation = Quaternion.Euler(dashTilt, transform.rotation.eulerAngles.y, wallRunCameraTilt);
        }

        private void StartDash()
        {
            //verändert die Script variablen
            dashing = true;
            usingGravity = false;
            noMove = true;
            move = Vector3.forward;
            //verändert die höhe des Spielers
            transform.localScale = new Vector3(transform.localScale.x, startScale.y * 0.75f, transform.localScale.z);
            //verändert den Drag
            rigid.drag = airDrag;
        }

        private void Dash()
        {
            //sorgt fürs sliden während des Dashens
            move = new Vector3(0, 0, 0);
            rigid.AddForce(-orientation.up * (gravity * 50 * Time.deltaTime), ForceMode.Impulse);
        }

        private void StopDash()
        {
            //setzt die Variablen wieder auf default
            dashing = false;
            transform.localScale = startScale;
            usingGravity = false;
            noMove = false;
        }
    }
}