using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    public class Elementselect : MonoBehaviour
    {
        public int fire;
        public int water;
        public int earth;
        public int air;
        public int plant;
        public int arcane;
        public int gold;
        public int dark;
        public int room;
        public Text text;
        public bool activated;
        private List<string> elementList;
        private int elementNumber = 1;
        private string selectedElement = "none";
        private int selectedElementLevel = 1;

        // Start is called before the first frame update
        void Start()
        {
            //erstellt eine Liste welche die ausgewählten Elemente enthält
            elementList = new List<string>();
            if (fire > 0) elementList.Add("Fire%" + fire);
            if (water > 0) elementList.Add("Water%" + water);
            if (earth > 0) elementList.Add("Earth%" + earth);
            if (air > 0) elementList.Add("Air%" + air);
            if (plant > 0) elementList.Add("Plant%" + plant);
            if (arcane > 0) elementList.Add("Arcane%" + arcane);
            if (gold > 0) elementList.Add("Gold%" + gold);
            if (dark > 0) elementList.Add("Dark%" + dark);
            if (room > 0) elementList.Add("Room%" + room);
            activated = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!activated) return;
            //Scroll durch die "Hotbar"
            if (Input.mouseScrollDelta.y <= -1f) --elementNumber;
            if (elementNumber <= 0) elementNumber = elementList.Count;
            if (Input.mouseScrollDelta.y >= 1f) ++elementNumber;
            if (elementNumber > elementList.Count) elementNumber = 1;

// Manuelles auswählen der Elmente
            if (Input.GetKeyDown(KeyCode.Alpha1) && elementList.Count >= 1) elementNumber = 1;
            if (Input.GetKeyDown(KeyCode.Alpha2) && elementList.Count >= 2) elementNumber = 2;
            if (Input.GetKeyDown(KeyCode.Alpha3) && elementList.Count >= 3) elementNumber = 3;
            if (Input.GetKeyDown(KeyCode.Alpha4) && elementList.Count >= 4) elementNumber = 4;
            if (Input.GetKeyDown(KeyCode.Alpha5) && elementList.Count >= 5) elementNumber = 5;
            if (Input.GetKeyDown(KeyCode.Alpha6) && elementList.Count >= 6) elementNumber = 6;
            if (Input.GetKeyDown(KeyCode.Alpha7) && elementList.Count >= 7) elementNumber = 7;
            if (Input.GetKeyDown(KeyCode.Alpha8) && elementList.Count >= 8) elementNumber = 8;
            if (Input.GetKeyDown(KeyCode.Alpha9) && elementList.Count >= 9) elementNumber = 9;
//Die Alpha Tasten sien die normalen nummern oben
            //auswählen des Elemntes
            selectedElement = elementList[elementNumber - 1];
            Elmentanzeige();
        }

        public bool IsElementActive(string elementNeeded)
        {
            if (!activated) return false;
            //schaut ob das nötige Elment ausgewählt wurde
            return elementNeeded ==
                   selectedElement.Split('%')[0];
        }

        public bool IsElementActive(string[] elementNeeded)
        {
            if (!activated) return false;
            //schaut ob das nötige Elment ausgewählt wurde
            foreach (string s in elementNeeded.Where(s => s == selectedElement.Split('%')[0]))
            {
                return true;
            }

            return false;
        }

        private void Elmentanzeige()
        {
            text.text = selectedElement.Split('%')[0] + " Level: " + selectedElement.Split('%')[1];
        }

        public int GetElementLevel(string element)
        {
            if (!activated) return 0;
            //gibt elemntlevel für spezifisches elment wieder
            foreach (var e in elementList.Where(e => e.Split('%')[0] == element))
            {
                int.TryParse(e.Split('%')[1], out selectedElementLevel);
            }

            return selectedElementLevel;
        }

        public int GetElementLevel()
        {
            if (!activated) return 0;
            int.TryParse(selectedElement.Split('%')[1], out selectedElementLevel);

            return selectedElementLevel;
        }
    }
}