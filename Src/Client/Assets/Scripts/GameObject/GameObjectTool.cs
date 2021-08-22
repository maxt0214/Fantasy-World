using UnityEngine;
using SkillBridge.Message;
using Entities;

public static class GameObjectTool
{
    public static Vector3 LogicUnitToWorld(NVector3 v)
    {
        return new Vector3(v.X / 100f, v.Z / 100f, v.Y / 100f);
    }

    public static Vector3 LogicUnitToWorld(Vector3Int v)
    {
        return new Vector3(v.x / 100f, v.z / 100f, v.y / 100f);
    }

    public static float LogicUnitToWorld(int val)
    {
        return val / 100f;
    }

    public static int WorldUnitToLogic(float val)
    {
        return Mathf.RoundToInt(val * 100);
    }

    public static NVector3 WorldUnitToLogicN(Vector3 v)
    {
        return new NVector3
        {
            X = Mathf.RoundToInt(v.x * 100),
            Y = Mathf.RoundToInt(v.z * 100),
            Z = Mathf.RoundToInt(v.y * 100)
        };
    }

    public static Vector3Int WorldUnitToLogicInt(Vector3 v)
    {
        return new Vector3Int
        {
            x = Mathf.RoundToInt(v.x * 100),
            y = Mathf.RoundToInt(v.z * 100),
            z = Mathf.RoundToInt(v.y * 100)
        };
    }

    public static NVector3 ToLogicN(this Vector3Int v)
    {
        return new NVector3()
        {
            X = v.x,
            Y = v.y,
            Z = v.z
        };
    }

    public static Vector3Int ToLogicInt(this NVector3 v)
    {
        return new Vector3Int
        {
            x = v.X,
            y = v.Y,
            z = v.Z
        };
    }

    public static bool UpdateEntity(NEntity entity, Vector3 pos, Quaternion rotation, float spd)
    {
        bool updated = false;

        var nPos = WorldUnitToLogicN(pos);
        var nDir = WorldUnitToLogicN(rotation.eulerAngles);
        var speed = WorldUnitToLogic(spd);

        if(!entity.Position.Equal(nPos))
        {
            entity.Position = nPos;
            updated = true;
        }

        if (!entity.Direction.Equal(nDir))
        {
            entity.Direction = nDir;
            updated = true;
        }

        if(entity.Speed != speed)
        {
            entity.Speed = speed;
            updated = true;
        }

        return updated;
    }
}
