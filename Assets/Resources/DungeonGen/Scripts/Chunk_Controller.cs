using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Chunk_Controller : MonoBehaviour
{
    //SPAWN TEST ROAD TILE.
    Map_Conditions map;

    //list of connecting rooms
    public List<Sibling> siblings;

    public Dictionary<Vector3Int, FloorTile> roadTiles;


    //used only if room shouldn be constructed from multiple tiles. can be ignored if only one room.
    public Dictionary<Vector3Int, FloorTile> floorplan;

    //position in world index
    public Vector2Int mapPos;

    List<int> entryCodes;
    
    //code, based off of number of entrances. Determines room layout
    public int chunkCode;

    //code based off of what chunkCode needs to become to fit into environment
    public int worldCode;

    public bool isVoid;
    public int roadCode;
    public bool hasRoad;

    Quaternion worldRot;

    public void GenerateMap()
    {
        chunkCode = GetCode();

        //clear any existing data
        floorplan.Clear();

        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }


        for (int x = 0; x < map.tileset.res; x++)
        {
            for (int y = 0; y < map.tileset.res; y++)
            {
                Vector3Int samplePos = new Vector3Int(x, 0, y);

                floorplan.Add(samplePos, new FloorTile());
            }
        }

        worldRot = GetWorldRot();

        AssignFloorplan();

        PopulateMap();
    }

    void PopulateMap()
    {

        foreach (Vector3Int v in floorplan.Keys)
        {
            Transform roomShell = null;
            GameObject b = null;

            
            


            //test if center of room by checking if tile is at the midpoint of floorplan
            bool isCenter = false;

            if (CheckRoadAlignment(v)) 
                isCenter = true;

            if (floorplan[v].entryType != FloorTile.EntryType.lvlExit)
            {
                if (!isVoid)
                {
                    //select which pool to get tiles
                    int poolIndex = 0;

                    if (!isCenter)
                    {
                        if (floorplan[v].forSibling)
                            poolIndex = 1;
                    }
                    else
                    {
                        poolIndex = 2;
                    }

                    Transform tilePool = map.tileset.tileMaster.transform.GetChild(poolIndex);

                    Debug.Log(tilePool.gameObject.name);

                    //loop through tiles in tilepool and grab archetype with matching opencode
                    if (poolIndex == 0)
                    {
                        for (int i = 0; i < tilePool.childCount; i++)
                        {
                            Transform t = tilePool.GetChild(i);


                            int useCode = map.tileset.res > 1 ? floorplan[v].openCode : chunkCode;
                            //Debug.Log(useCode);

                            if (int.Parse(t.gameObject.name) == useCode)
                            {
                                roomShell = t;
                            }
                        }
                    }
                    else
                    {
                        Transform subPool = tilePool.GetChild(floorplan[v].subIndex);

                        for (int i = 0; i < subPool.childCount; i++)
                        {
                            Transform t = subPool.GetChild(i);


                            int useCode = map.tileset.res > 1 ? floorplan[v].openCode : chunkCode;

                            //Debug.Log(useCode);
                            if (int.Parse(t.gameObject.name) == useCode)
                            {
                                roomShell = t;
                            }
                        }
                    }
                }
                else
                {
                    roomShell = map.tileset.tileMaster.transform.GetChild(0).GetChild(0);
                }

               

                int rI = UnityEngine.Random.Range(0, roomShell.childCount);
                b = Instantiate(roomShell.GetChild(rI).gameObject);
            }
            else
            {
                foreach (Transform t in map.tileset.tileMaster.transform)
                {
                    GameObject g = t.gameObject;
                    if (g.name.Contains("Exit"))
                    {
                        b = Instantiate(g);
                    }
                }
            }

            //load room and place into world

              
            b.transform.parent = transform;

            b.name = $"tile_{transform.childCount}_{v.ToString()}_{floorplan[v].openCode}_{floorplan[v].roomCode}_{floorplan[v].entryType.ToString()}";

            var tile = map.tileset;
            Vector3 centerOff = ((new Vector3(1, 0, 1) * (tile.tileSize * tile.res)) / 2) - new Vector3(tile.tileSize / 2, 0, tile.tileSize / 2);

            b.transform.position = (transform.position - centerOff) + new Vector3(tile.tileSize * v.x, 0, tile.tileSize * v.z);

            if(map.tileset.res > 1)
                GetRotation(v, b.transform);
            
        }

        
        if(map.alignChunks)
            transform.rotation = worldRot;
        
    }

    void GetRotation(Vector3Int v, Transform t)
    {
       
        var f = floorplan[v];

        if (f.openCode != 15)
        {
            if (!floorplan[v].isRoad)
            {
                for (int i = 90; i < 360; i += 90)
                {
                    for (int e = 0; e < f.entryPositions.Count; e++)
                    {
                        f.entryPositions[e] *= 2;

                        if (f.entryPositions[e] > 8)
                            f.entryPositions[e] = 1;

                        //add all codes together to get room code
                        int testCode = 0;

                        foreach (int c in floorplan[v].entryPositions)
                        {
                            testCode += c;
                        }

                        if (testCode == f.roomCode)
                        {
                            Quaternion rot = Quaternion.Euler(0, i, 0);
                            t.rotation = rot;

                            break;
                        }
                    }
                }
            }
            else
            {
                Vector3Int centerPoint = new Vector3Int((int)map.tileset.res / 2, 0, (int)map.tileset.res / 2);
                float rot = 0;

                if (v == centerPoint)
                {
                    switch (floorplan[v].openCode)
                    {
                        case 1:
                            switch (floorplan[v].roomCode)
                            {
                                case 2:
                                    rot = 90;
                                    break;
                                case 4:
                                    rot = 180;
                                    break;
                                case 8:
                                    rot = 270;
                                    break;
                            }
                            break;
                        case 7:
                            switch (floorplan[v].roomCode)
                            {
                                case 14:
                                    rot = 90;
                                    break;
                                case 13:
                                    rot = 180;
                                    break;
                                case 11:
                                    rot = 270;
                                    break;
                            }
                            break;
                        case 3:
                            switch (floorplan[v].roomCode)
                            {
                                case 6:
                                    rot = 90;
                                    break;
                                case 12:
                                    rot = 180;
                                    break;
                                case 9:
                                    rot = 270;
                                    break;
                            }
                            break;

                    }
                }
                else
                {
                    if (floorplan[v].openCode == 1)
                    {
                        switch (floorplan[v].roomCode)
                        {
                            case 1:
                                rot = 0;
                                break;
                            case 2:
                                rot = 270;
                                break;
                            case 4:
                                rot = 180;
                                break;
                            case 8:
                                rot = 90;
                                break;
                        }

                    }
                    else
                    {
                        if (v.x != centerPoint.x)
                            rot = 90 * Mathf.Sign(v.x - centerPoint.x);
                    }
                }

                t.rotation = Quaternion.Euler(0, rot, 0);
            }
        }
        else
        {

            int rStart = UnityEngine.Random.Range(0, 4);
            float rAngle = 0;

            for (int i = rStart; i < 4; i++)
            {
                rAngle += 90;
            }

            Quaternion rot = Quaternion.Euler(0, rAngle, 0);
            t.rotation = rot;
        }

    }

    bool CheckRoadAlignment(Vector3Int startPos)
    {
        bool test = false;

        Vector3Int centerPoint = new Vector3Int((int)map.tileset.res / 2, 0, (int)map.tileset.res / 2);

        bool centerX = startPos.x == centerPoint.x;
        bool hasX = false;
        bool centerY = startPos.z == centerPoint.z;
        bool hasY = false;
        bool isEntry = false;
        bool deadEntry = false;

        int eCount = 0;
        int newRoomCode = 0;

        if (centerY)
        {
            
            for (int x = 0; x < map.tileset.res; x++)
            {
                Vector3Int checkX = new Vector3Int(x, 0, startPos.z);

                bool checkInside = floorplan.ContainsKey(checkX);
                bool checkType = checkInside ? (floorplan[checkX].entryType == FloorTile.EntryType.road || floorplan[checkX].entryType == FloorTile.EntryType.roadJoint) : true;

                if (checkInside && !deadEntry)
                {
                    if (floorplan[checkX].entryType == FloorTile.EntryType.road || floorplan[checkX].entryType == FloorTile.EntryType.roadJoint || floorplan[checkX].entryType == FloorTile.EntryType.roadEnd)
                    {
                        Vector3 entryDir = checkX - centerPoint;
                        Vector3 checkDir = startPos - centerPoint;

                        hasY = true;

                        if (checkX.x < centerPoint.x)
                            newRoomCode += 8;
                        else
                            newRoomCode += 2;

                        eCount++;



                        if (Vector3.Angle(entryDir, checkDir) < 90)
                            test = true;
                    }
                }

            }
        }
        
        if(centerX)
        { 
            for (int y = 0; y < map.tileset.res; y++)
            {
                Vector3Int checkY = new Vector3Int(startPos.x, 0, y);

                bool checkInside = floorplan.ContainsKey(checkY);
                bool checkType = checkInside ? (floorplan[checkY].entryType == FloorTile.EntryType.road || floorplan[checkY].entryType == FloorTile.EntryType.roadJoint) : true;

                if (checkInside)
                {
                    if (floorplan[checkY].entryType == FloorTile.EntryType.road || floorplan[checkY].entryType == FloorTile.EntryType.roadJoint || floorplan[checkY].entryType == FloorTile.EntryType.roadEnd)
                    {
                        Vector3 entryDir = checkY - centerPoint;
                        Vector3 checkDir = startPos - centerPoint;

                        hasX = true;

                        if (checkY.z < centerPoint.z)
                            newRoomCode += 4;
                        else
                            newRoomCode += 1;

                        eCount++;

                        if (Vector3.Angle(entryDir, checkDir) < 90)
                            test = true;
                    }
                }
            }
        }

        /**/
        if (floorplan[startPos].entryType == FloorTile.EntryType.roadEnd)
        {
            if (centerY)
            {
                if (startPos.x < centerPoint.x)
                    newRoomCode = 8;
                else
                    newRoomCode = 2;
            }
            else
            {
                if (startPos.z < centerPoint.z)
                    newRoomCode = 1;
                else
                    newRoomCode = 4;
            }
        }
        

        if(test)
        {
            floorplan[startPos].roomCode = newRoomCode;
            floorplan[startPos].subIndex = roadCode;
            floorplan[startPos].isRoad = true;

            if (startPos != centerPoint)
            {
                if (floorplan[startPos].entryType == FloorTile.EntryType.roadJoint)
                {
                    foreach (Sibling s in siblings)
                    {
                        //check to see if entry should be converted to other type
                        Vector3 eOff = worldRot * (startPos - centerPoint);
                        Vector3 sibOff = new Vector3(s.offset.x, 0, s.offset.y);

                        float eDot = Vector3.Angle(sibOff, eOff);

                        if (s.connectRoad)
                            isEntry = true;
                    }

                    
                }
                else if (floorplan[startPos].entryType == FloorTile.EntryType.roadEnd)
                {
                    floorplan[startPos].openCode = 1;
                }

                if(floorplan[startPos].entryType != FloorTile.EntryType.roadEnd)
                    floorplan[startPos].openCode = 5;

            }
            else
            {

                switch (eCount)
                {
                    case 1:
                        floorplan[startPos].openCode = 1;
                        break;
                    case 2:
                        if (hasX && hasY)
                            floorplan[startPos].openCode = 3;
                        else
                            floorplan[startPos].openCode = 5;
                        break;
                    case 3:
                        floorplan[startPos].openCode = 7;
                        break;
                    case 4:
                        floorplan[startPos].openCode = 15;
                        break;
                }
            }

            roadTiles.Add(startPos, floorplan[startPos]);
        }


        return test;
    }

    Quaternion GetWorldRot()
    {
        Quaternion test = transform.rotation;
        worldCode = GetWorldCode(this);

       

        if (chunkCode != 15)
        {
            for (int i = 90; i < 360; i += 90)
            {

                for (int e = 0; e < entryCodes.Count; e++)
                {
                    entryCodes[e] *= 2;

                    if (entryCodes[e] > 8)
                        entryCodes[e] = 1;

                    int testCode = GetSibCode();

                    if (testCode == worldCode)
                    {
                        Quaternion rot = Quaternion.Euler(0, i, 0);
                        test = rot;
                    }
                }
            }
        }
        else
        {

            int rStart = UnityEngine.Random.Range(0, 4);
            float rAngle = 0;

            for (int i = rStart; i < 4; i++)
            {
                rAngle += 90;
            }

            Quaternion rot = Quaternion.Euler(0, rAngle, 0);
            test = rot;
            

        }

        return test;
    }

    int GetCode()
    {
        if (siblings.Count > 0)
        {
            if(siblings.Count == 2)
            {
                //if there are only two entrances, check angle and determine if its corner or hallway
                Vector2Int pos1 = siblings[0].chunk.mapPos - mapPos;
                Vector2Int pos2 = siblings[1].chunk.mapPos - mapPos;

                float angle = Vector2.Angle(pos1, pos2);

                if (angle > 90)
                {
                    entryCodes.Add(1);
                    entryCodes.Add(4);
                }
                else
                {
                    entryCodes.Add(1);
                    entryCodes.Add(2);
                }
            }
            else
            {
                //add up total number of siblings
                for (int i = 0; i < siblings.Count; i++)
                {
                    entryCodes.Add((int)Mathf.Pow(2, i));
                }
            }

        }



        return GetSibCode();
        
    }

    void AssignFloorplan()
    {
        for (int x = 0; x < map.tileset.res; x++)
        {
            for (int y = 0; y < map.tileset.res; y++)
            {
                Vector3Int v = new Vector3Int(x, 0, y);
                

                if (floorplan.ContainsKey(v))
                {

                    //determine if tile shoudl be an entrance
                    if ((y == 0 || y == map.tileset.res - 1))
                    {
                        if (chunkCode != 3 && chunkCode != 1)
                        {
                            //if vertical column
                            if ((chunkCode & 5) > 0)
                            {

                                if((x > 0 && (y > 0 && y < map.tileset.res - 1)) || (x > 0 && x < map.tileset.res - 1) || (chunkCode == 7 && x > 0) || chunkCode == 15)
                                    floorplan[v].isBorder = true;

                                if (x == (int)(map.tileset.res / 2))
                                    floorplan[v].entryType = FloorTile.EntryType.entry;
                            }
                        }
                        else
                        {
                            //if corner or single entrance
                            if (y == map.tileset.res - 1)
                            {

                                if(((chunkCode == 3 || chunkCode == 1) && (x > 0 && x < map.tileset.res - 1)) || (chunkCode == 3 && y == map.tileset.res - 1 && x > 0))
                                    floorplan[v].isBorder = true;

                                if (x == (int)(map.tileset.res / 2))
                                    floorplan[v].entryType = FloorTile.EntryType.entry;
                            }
                        }
                    }
                    else if (x == 0 || x == map.tileset.res - 1)
                    {

                        if (chunkCode != 7 && chunkCode != 3)
                        {
                            if ((chunkCode & 10) > 0)
                            {
                                if(chunkCode == 15)
                                    floorplan[v].isBorder = true;

                                if (y == (int)(map.tileset.res / 2))
                                    floorplan[v].entryType = FloorTile.EntryType.entry;
                            }
                        }
                        else
                        {
                            if (x == map.tileset.res - 1)
                            {
                                if(chunkCode == 7 || (y > 0 && y < map.tileset.res - 1))
                                    floorplan[v].isBorder = true;

                                if (y == (int)(map.tileset.res / 2))
                                    floorplan[v].entryType = FloorTile.EntryType.entry;
                            }
                        }
                        
                    }

                    //check to see if entry should be converted to other type
                    Vector3 centerPoint = new Vector3Int((int)map.tileset.res / 2, 0, (int)map.tileset.res / 2);
                    Vector3 eOff = worldRot * (v - centerPoint).normalized;


                    //determine which entrances should be road joints
                    if (floorplan[v].entryType == FloorTile.EntryType.entry)
                    {
                        foreach(Sibling sib in siblings)
                        {
                            Vector3 sibOff = new Vector3(sib.offset.x, 0, sib.offset.y);

                            float eDot = Vector3.Angle(sibOff, eOff);
                            sib.rScore = eDot;

                            if (eDot < 25)
                            {
                                //Debug.Log($"{mapPos}_{sib.offset}_{eDot}_{sib.connectRoad}_{eOff}_{v}");
                                //Debug.Log(sib.offset);

                                if (this != map.endChunk && this != map.startChunk)
                                {
                                    if (sib.connectRoad && sib.chunk.roadCode == roadCode)
                                        floorplan[v].entryType = FloorTile.EntryType.roadJoint;
                                    else if (!sib.connectRoad && sib.chunk.roadCode != roadCode || (sib.chunk == map.endChunk || sib.chunk == map.startChunk))
                                        floorplan[v].entryType = FloorTile.EntryType.roadEnd;
                                }
                            }
                            
                        }
                    }

                    if(this == map.endChunk && v == centerPoint)
                    {
                        floorplan[v].entryType = FloorTile.EntryType.lvlExit;
                    }


                    int eCount = 0;
                    //rotate through adjactent floor tiles, and if tile exists...
                    for (int i = 0; i < 4; i++)
                    {
                        float a = 90 * i;
                        int xR = (int)Mathf.Sin(a * Mathf.Deg2Rad);
                        int yR = (int)Mathf.Cos(a * Mathf.Deg2Rad);

                        Vector3Int dir = new Vector3Int(xR, 0, yR);

                        if (floorplan.ContainsKey(v + dir) && map.tileset.res > 1)
                        {
                            floorplan[v].roomCode += (int)Mathf.Pow(2, i);
                            eCount++;
                        }
                        else
                        {
                            if (floorplan[v].entryType == FloorTile.EntryType.entry)
                            {
                                floorplan[v].roomCode += (int)Mathf.Pow(2, i);
                                eCount++;
                            }

                            if (floorplan[v].isBorder)
                            {
                                if (!floorplan[v].forSibling && map.tileset.useSiblingTiles)
                                    floorplan[v].forSibling = true;
                            }
                        }
                    }

                    for (int i = 0; i < eCount; i++)
                    {
                        floorplan[v].openCode += (int)Mathf.Pow(2, i);
                        floorplan[v].entryPositions.Add((int)Mathf.Pow(2, i));
                    }
                }

            }
        }
    }

    bool SibPos(Vector3Int v)
    {
        bool test = false;

        Vector2Int pos = new Vector2Int(v.x, v.z);

        foreach(Sibling s in siblings)
        {
            if (s.chunk.mapPos == mapPos + pos)
                test = true;
        }

        return test;
    }

    int CountCode(Vector3Int v)
    {
        //add all codes together to get room code
        int i = 0;

        foreach(int c in floorplan[v].entryPositions)
        {
            i += c;
        }


        return i;
    }

    int GetSibCode()
    {
        //add all codes together to get room code
        int i = 0;

        foreach (int c in entryCodes)
        {
            i += c;
        }

        return i;
    }

    //THIS WAS POIINTLESSSSS
    public int GetWorldCode(Chunk_Controller targetChunk)
    {

        int testInc = 0;
        int test = 0;

        for (int i = 0; i < 4; i++)
        {
            float angle = testInc * Mathf.Deg2Rad;

            Vector2Int checkPos = new Vector2Int((int)Mathf.Sin(angle), (int)Mathf.Cos(angle));

            testInc += 90;

            foreach (Sibling s in targetChunk.siblings)
            {
                if (s.chunk.mapPos - targetChunk.mapPos == checkPos)
                {
                    test += (int)Mathf.Pow(2, i);
                }
            }
        }

        return test;
    }

    public void Init(Vector2Int m)
    {
        map = FindObjectOfType<Map_Conditions>();
        siblings = new List<Sibling>();
        entryCodes = new List<int>();
        floorplan = new Dictionary<Vector3Int, FloorTile>();
        roadTiles = new Dictionary<Vector3Int, FloorTile>();

        mapPos = m;
    }
    
    [Serializable]
    public class Sibling
    {
        public Chunk_Controller chunk;
        public Vector2Int offset;
        public int offCode;
        //determines general map for road
        public bool connectRoad;
        public float rScore;

        public Sibling(Chunk_Controller c, Vector2Int off)
        {
            chunk = c;

            offset = off;
        }
    }

    public class FloorTile
    {
        //code based on the number of entrances. Used to grab the type of tile
        public int openCode;
        //code used to determine where the openings are, to orient tile once it has been grabbned
        public int roomCode;
        //locations of individial entrances. starts being equal to open code, and then cycled/totalled and compared with roomcode to determine tile rotation
        public List<int> entryPositions;
        //should tile be entry?
        public enum EntryType
        {
            none,
            entry,
            road,
            roadJoint,
            roadEnd,
            lvlExit
        }
        public EntryType entryType;
        //does tile border sibling
        public bool isBorder;
        //is tile next to sibling?
        public bool forSibling;
        //index of sub tileset, if applicable
        public int subIndex;
        //is tile a road
        public bool isRoad;

        public FloorTile()
        {
            entryPositions = new List<int>();
        }
    }

}


