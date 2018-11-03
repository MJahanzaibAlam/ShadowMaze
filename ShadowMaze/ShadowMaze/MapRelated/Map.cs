namespace ShadowMaze
{
    // Stores the map name and star score achieved on the map.
    public class Map
    {
        private string strMapName; // Stores the name of the map.
        private int intStarScore; // Stores the star score for the map.

        // Creates a new map with the attributes name and score.
        public Map(string strName, int intScore)
        {
            strMapName = strName;
            intStarScore = intScore;
        }

        public string MapName
        {
            get { return strMapName; }
            set { strMapName = value; }
        }

        public int StarScore
        {
            get { return intStarScore; }
            set { intStarScore = value; }
        }
        
        // Checks if two maps are equal based on their names.
        public override bool Equals(object o)
        {
            if (o.GetType() != typeof(Map))
                return false;

            return (strMapName == ((Map)o).strMapName);
        }

        public override int GetHashCode()
        {
            return strMapName.GetHashCode();
        }
    }
}
