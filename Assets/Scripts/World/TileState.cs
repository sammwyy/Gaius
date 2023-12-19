using System.Collections.Generic;
using UnityEngine;

/**
    TileStatus is the status of a tile, which can be empty, building, occupied, or destroyed.
*/
public enum TileStatus
{
    Empty,
    Building,
    Occupied,
    Destroyed,
    Invalid
}

/**
    TileState is the state of a tile, containing all the stats of a tile based on its meta.
*/
public class TileState : ScriptableObject
{
    // Common stats.
    public float health = -1f;
    public int level = -1;
    public TileStatus status = TileStatus.Invalid;

    // Residential properties.
    public int population = -1;

    // Commercial properties.
    public int jobs = -1;

    // Industrial properties.
    public int resourceProductionRate = -1;
    public int resourceConsumptionRate = -1;

    // World properties.
    public List<TileWorldSlot> educationNearby = new();
    public List<TileWorldSlot> firemanNearby = new();
    public List<TileWorldSlot> hospitalNearby = new();
    public List<TileWorldSlot> parksNearby = new();
    public List<TileWorldSlot> policeNearby = new();
    public List<TileWorldSlot> religiousNearby = new();
    public List<TileWorldSlot> roadsNearby = new();
    public List<TileWorldSlot> waterSupplyNearby = new();

    // Getters: World properties.
    public bool HasEducationNearby()
    {
        return this.educationNearby.Count > 0;
    }

    public bool HasFiremanNearby()
    {
        return this.firemanNearby.Count > 0;
    }

    public bool HasHospitalNearby()
    {
        return this.hospitalNearby.Count > 0;
    }

    public bool HasParksNearby()
    {
        return this.parksNearby.Count > 0;
    }

    public bool HasPoliceNearby()
    {
        return this.policeNearby.Count > 0;
    }

    public bool HasReligiousNearby()
    {
        return this.religiousNearby.Count > 0;
    }

    public bool HasRoadsNearby()
    {
        return this.roadsNearby.Count > 0;
    }

    public bool HasWaterSupplyNearby()
    {
        return this.waterSupplyNearby.Count > 0;
    }

    // Utils methods.
    public void Reset()
    {
        this.status = TileStatus.Invalid;
        this.health = 0;
        this.level = 0;
        this.population = 0;
        this.jobs = 0;
        this.resourceConsumptionRate = 0;
        this.resourceProductionRate = 0;
    }
}