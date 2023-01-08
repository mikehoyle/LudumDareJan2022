using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game {
  public class GameController : MonoBehaviour {
    [SerializeField] private Tile wheatTile;
    [SerializeField] private Tile pumpkinTile;
    [SerializeField] private Tile cornTile;
    [SerializeField] private Tile tomatoTile;
    [SerializeField] private Tile grapeTile;
    [SerializeField] private Tile emptyWheatTile;
    [SerializeField] private Tile emptyPumpkinTile;
    [SerializeField] private Tile emptyCornTile;
    [SerializeField] private Tile emptyTomatoTile;
    [SerializeField] private Tile emptyGrapeTile;

    private Tilemap _baseTilemap;
    private HashSet<Vector3Int> _collectedCrops = new();

    private void Awake() {
      _baseTilemap = GameObject.FindWithTag("BaseTilemap").GetComponent<Tilemap>();
    }

    private void Start()
    {


      string farmerPath = "Assets/Prefabs/EnemyFarmer.prefab";
      string dogPath = "Assets/Prefabs/EnemyDog.prefab";
      var farmerPrefab = PrefabUtility.LoadPrefabContents(farmerPath);
      var dogPrefab = PrefabUtility.LoadPrefabContents(dogPath);
      Instantiate(farmerPrefab);
      Instantiate(dogPrefab);
    }

    public CropType GetCropAtWorldPosition(Vector3 position) {
      var coordsUnderCar = _baseTilemap.WorldToCell(position);
      if (_collectedCrops.Contains(coordsUnderCar)) {
        return CropType.None;
      }
      
      var tile = _baseTilemap.GetTile<Tile>(coordsUnderCar);
      if (tile == wheatTile) {
        return CropType.Wheat;
      }

      if (tile == pumpkinTile) {
        return CropType.Pumpkins;
      }

      if (tile == cornTile) {
        return CropType.Corn;
      }

      if (tile == tomatoTile) {
        return CropType.Tomato;
      }
      
      if (tile == grapeTile) {
        return CropType.Grape;
      }

      return CropType.None;
    }

    public void CollectCropAtWorldPosition(Vector3 position, CropType type) {
      var coords = _baseTilemap.WorldToCell(position);
      if (type == CropType.Corn) {
        _baseTilemap.SetTile(coords, emptyCornTile); 
      } else if (type == CropType.Pumpkins) {
        _baseTilemap.SetTile(coords, emptyPumpkinTile); 
      } else if (type == CropType.Wheat) {
        _baseTilemap.SetTile(coords, emptyWheatTile); 
      } else if (type == CropType.Tomato) {
        _baseTilemap.SetTile(coords, emptyTomatoTile);
      } else if (type == CropType.Grape) {
        _baseTilemap.SetTile(coords, emptyGrapeTile);
      }
      // TODO: crop regeneration
      _collectedCrops.Add(_baseTilemap.WorldToCell(position));
    }
  }
}
