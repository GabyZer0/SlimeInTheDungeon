using System;
using AKSaigyouji.ArrayExtensions;
using UnityEngine;
using AKSaigyouji.CaveGeneration;
using AKSaigyouji.MeshGeneration;
using AKSaigyouji.HeightMaps;
using AKSaigyouji.Maps;
using AKSaigyouji.Modules.CaveWalls;
using AKSaigyouji;


public class MyCaveGenerator : CaveGenerator
{
    private ThreeTierCaveConfiguration config;

    private MeshGenerator meshGenerator = new MeshGenerator();
        
    public MyCaveGenerator(ThreeTierCaveConfiguration ttcc)
    {
        config = ttcc;
    }

    public override GameObject Generate()
    {
        Map map = config.MapGenerator.Generate();   
        
        IHeightMap floor = config.FloorHeightMapModule.GetHeightMap();
        IHeightMap ceiling = config.CeilingHeightMapModule.GetHeightMap();
        CaveWallModule caveWalls = config.WallModule;

        Map[,] mapChunks = MapSplitter.Subdivide(map);
        CaveMeshes[,] caveChunks = GenerateCaveChunks(mapChunks, config.CaveType, config.Scale, floor, ceiling, caveWalls);
        ThreeTierCave cave = new ThreeTierCave(caveChunks);
        AssignMaterials(cave, config.FloorMaterial, config.WallMaterial, config.CeilingMaterial);

        return cave.GameObject;
    }

    CaveMeshes[,] GenerateCaveChunks(Map[,] mapChunks, ThreeTierCaveType type, int scale,
            IHeightMap floor, IHeightMap ceiling, CaveWallModule walls)
    {
        int xNumChunks = mapChunks.GetLength(0);
        int yNumChunks = mapChunks.GetLength(1);
        var caveChunks = new CaveMeshes[xNumChunks, yNumChunks];
        var actions = new Action[mapChunks.Length];
        mapChunks.ForEach((x, y) =>
        {
            Coord index = new Coord(x, y);
            actions[y * xNumChunks + x] = new Action(() =>
            {
                WallGrid grid = MapConverter.MapToWallGrid(mapChunks[x, y], scale, index);
                MeshData floorMesh = meshGenerator.BuildFloor(grid, floor);
                MeshData ceilingMesh = SelectCeilingBuilder(type)(grid, ceiling);
                MeshData wallMesh = meshGenerator.BuildWalls(grid, floor, ceiling, walls);

                caveChunks[index.x, index.y] = new CaveMeshes(floorMesh, wallMesh, ceilingMesh);
            });
        });
        Execute(actions);
        return caveChunks;
    }

    void AssignMaterials(ThreeTierCave cave, Material floorMat, Material wallMat, Material ceilingMat)
    {
        foreach (var floor in cave.GetFloors())
        {
            floor.Material = floorMat;
        }
        foreach (var wall in cave.GetWalls())
        {
            wall.Material = wallMat;
        }
        foreach (var ceiling in cave.GetCeilings())
        {
            ceiling.Material = ceilingMat;
        }
    }

    Func<WallGrid, IHeightMap, MeshData> SelectCeilingBuilder(ThreeTierCaveType caveType)
    {
        switch (caveType)
        {
            case ThreeTierCaveType.Isometric:
                return meshGenerator.BuildCeiling;
            case ThreeTierCaveType.Enclosed:
                return meshGenerator.BuildEnclosure;
            default:
                throw new System.ComponentModel.InvalidEnumArgumentException();
        }
    }
}
