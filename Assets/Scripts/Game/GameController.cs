using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game {
  public class GameController : MonoBehaviour {
    [SerializeField] private Tile wheatTile;
    [SerializeField] private Tile pumpkinTile;
    [SerializeField] private Tile cornTile;
    [SerializeField] private Tile emptyWheatTile;
    [SerializeField] private Tile emptyPumpkinTile;
    [SerializeField] private Tile emptyCornTile;

    private Tilemap _baseTilemap;
    private HashSet<Vector3Int> _collectedCrops = new();

    private void Awake() {
      _baseTilemap = GameObject.FindWithTag("BaseTilemap").GetComponent<Tilemap>();
    }

    public Tile GetTileForType(CropType cropType) {
      switch (cropType) {
        case CropType.Corn:
          return cornTile;
        case CropType.Pumpkins:
          return pumpkinTile;
        case CropType.Wheat:
          return wheatTile;
        case CropType.None:
        default:
          return null;
      }
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
      }
      // TODO: crop regeneration
      _collectedCrops.Add(_baseTilemap.WorldToCell(position));
    }
  }
}
