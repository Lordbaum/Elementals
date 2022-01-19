using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace MovmentUndSo
{
    public class ServerPlayerControll : NetworkBehaviour
    {
        public bool w;
        public bool a;
        public bool s;
        public bool d;
        public bool sprint;
        public bool crouch;
        public bool jump;
        public bool defend;
        public bool prime;
        public bool second;

        private readonly List<KeyValuePair<string, int>> buttonList = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("W", 0),
            new KeyValuePair<string, int>("A", 1),
            new KeyValuePair<string, int>("S", 2),
            new KeyValuePair<string, int>("D", 3),
            new KeyValuePair<string, int>("Sprint", 4),
            new KeyValuePair<string, int>("Crouch", 5),
            new KeyValuePair<string, int>("Jump", 6),
            new KeyValuePair<string, int>("Defend", 7),
            new KeyValuePair<string, int>("Prime", 8),
            new KeyValuePair<string, int>("Second", 9)
        };

        NetworkList<bool> inputList = new NetworkList<bool>();

        void Start()
        {
            inputList.Add(w);
            inputList.Add(a);
            inputList.Add(s);
            inputList.Add(d);
            inputList.Add(sprint);
            inputList.Add(crouch);
            inputList.Add(jump);
            inputList.Add(defend);
            inputList.Add(prime);
            inputList.Add(second);
        }

        // Update is called once per frame
        void Update()
        {
            if (!GetComponent<NetworkObject>().IsLocalPlayer) return;
            //Gets the input and turns into bools
            w = Input.GetKey(KeyCode.W);
            a = Input.GetKey(KeyCode.A);
            s = Input.GetKey(KeyCode.S);
            d = Input.GetKey(KeyCode.D);
            sprint = Input.GetKey(KeyCode.LeftShift);
            crouch = Input.GetKey(KeyCode.LeftControl);
            jump = Input.GetKey(KeyCode.Space);
            defend = Input.GetKey(KeyCode.E);
            prime = Input.GetKey(KeyCode.Mouse0);
            second = Input.GetKey(KeyCode.Mouse1);
            //send the bools to the Server
            InputSyncServerRpc();
        }

        [ServerRpc]
        public void InputSyncServerRpc()
        {
            inputList[0] = w;
            inputList[1] = a;
            inputList[2] = s;
            inputList[3] = d;
            inputList[4] = sprint;
            inputList[5] = crouch;
            inputList[6] = jump;
            inputList[7] = defend;
            inputList[8] = prime;
            inputList[9] = second;
        }

        public bool GetInput(string Move)
        {
            int i = 0;
            foreach ((string key, int value) in buttonList)
            {
                if (key == Move)
                {
                    i = value;
                    break;
                }
            }

            return inputList[i];
        }
    }
}