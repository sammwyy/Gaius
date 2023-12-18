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
    public List<Tile> educationNearby = new();
    public List<Tile> firemanNearby = new();
    public List<Tile> hospitalNearby = new();
    public List<Tile> parksNearby = new();
    public List<Tile> policeNearby = new();
    public List<Tile> religiousNearby = new();
    public List<Tile> roadsNearby = new();
    public List<Tile> waterSupplyNearby = new();

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