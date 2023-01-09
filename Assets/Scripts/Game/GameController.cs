using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [SerializeField] private GameObject farmerPrefab;
    [SerializeField] private GameObject dogPrefab;
    

    // TODO: these should be random or scale with time
    [SerializeField] private int requestSecs;
    [SerializeField] private int requestFrequencySecs;
    [SerializeField] private float cropRegrowthTimeSecs;

    private float _secsUntilNextRequest;
    private Tilemap _baseTilemap;
    // Maps growing crops to growth time remaining
    private readonly Dictionary<Vector3Int, float> _regrowingCrops = new();
    private MarketStand[] _marketStands;
    private int _score;
    private Text _scoreIndicator;
    private int _missedOrders;
    private Text _missedOrdersIndicator;

    public ResourceRequest[] OutstandingRequests { get; private set; }

    private void Awake() {
      _baseTilemap = GameObject.FindWithTag("BaseTilemap").GetComponent<Tilemap>();
    }

    private void Start() {
      LoadEnemies();
      _marketStands =
          GameObject.FindGameObjectsWithTag("MarketStand")
              .Select(x => x.GetComponent<MarketStand>()).ToArray();
      OutstandingRequests = new ResourceRequest[_marketStands.Length];
      _secsUntilNextRequest = requestFrequencySecs;
      _scoreIndicator = GameObject.FindWithTag("Score").GetComponent<Text>();
      _missedOrdersIndicator = GameObject.FindWithTag("MissedOrders").GetComponent<Text>();
      
      // Always start with corn
      SpawnNewResourceRequest(CropType.Corn);
    }

    private void Update() {
      UpdateResourceRequests();
      MaybeAddResourceRequest();
      UpdateCropRegrowth();
      _scoreIndicator.text = $"Score: {_score}";
      _missedOrdersIndicator.text = $"Missed Orders: {_missedOrders}";
    }

    private void UpdateCropRegrowth() {
      var growingCropKeys = new List<Vector3Int>(_regrowingCrops.Keys);
      foreach (var cropTile in growingCropKeys) {
        _regrowingCrops[cropTile] -= Time.deltaTime;
        if (_regrowingCrops[cropTile] <= 0) {
          _baseTilemap.SetTile(cropTile, GetTileForCrop(cropTile));
          _regrowingCrops.Remove(cropTile);
        }
      }
    }

    private void MaybeAddResourceRequest() {
      _secsUntilNextRequest -= Time.deltaTime;
      if (_secsUntilNextRequest <= 0) {
        // TODO game over here if too many requests?
        SpawnNewResourceRequest();
        _secsUntilNextRequest = requestFrequencySecs;
      }
    }

    private void UpdateResourceRequests() {
      for (int i = 0; i < OutstandingRequests.Length; i++) {
        if (OutstandingRequests[i] != null) {
          OutstandingRequests[i].Update();
          _marketStands[i]
              .UpdateVisuals(
                  OutstandingRequests[i].RequestedResource, OutstandingRequests[i].TimeRemainingSecs);
          if (OutstandingRequests[i].TimeRemainingSecs <= 0f) {
            OutstandingRequests[i] = null;
            _missedOrders++;
            _score -= requestSecs;
            // TODO actually handle a request expiring
            //    in a way that is meaningful.
          }
        } else {
          _marketStands[i].UpdateVisuals(CropType.None, 0f);
        }
      }
    }

    private void SpawnNewResourceRequest(CropType cropType = CropType.None) {
      var availableMarkets = new List<int>();
      for (var i = 0; i < _marketStands.Length; i++) {
        if (OutstandingRequests[i] == null) {
          availableMarkets.Add(i);
        }
      }

      if (availableMarkets.Count == 0) {
        return;
      }

      var requester = Random.Range(0, availableMarkets.Count);
      if (cropType == CropType.None) {
        cropType = (CropType)Random.Range(1, (int)CropType.Grape);
      }
      OutstandingRequests[availableMarkets[requester]] = new ResourceRequest(
          availableMarkets[requester], requestSecs, cropType);
    }

    private void LoadEnemies() {
      Instantiate(farmerPrefab);
      // Dog isn't working well enough to include for now
      //Instantiate(dogPrefab);
    }

    public CropType GetCropAtWorldPosition(Vector3 position) {
      var coordsUnderCar = _baseTilemap.WorldToCell(position);
      if (_regrowingCrops.Keys.Contains(coordsUnderCar)) {
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
      
      _regrowingCrops.Add(
          _baseTilemap.WorldToCell(position), cropRegrowthTimeSecs);
    }

    private Tile GetTileForCrop(Vector3Int coords) {
      var tile = _baseTilemap.GetTile<Tile>(coords);
      if (tile == emptyCornTile) {
        return cornTile;
      }

      if (tile == emptyPumpkinTile) {
        return pumpkinTile;
      }

      if (tile == emptyGrapeTile) {
        return grapeTile;
      }

      if (tile == emptyTomatoTile) {
        return tomatoTile;
      }
    
      if (tile == emptyWheatTile) {
        return wheatTile;
      }

      return wheatTile;
    }

    public ResourceRequest GetRequestForMarketStand(GameObject marketObject) {
      var marketStand = marketObject.GetComponent<MarketStand>();
      if (marketStand == null) {
        return null;
      }
      
      for (int i = 0; i < _marketStands.Length; i++) {
        if (_marketStands[i] != null && marketStand == _marketStands[i]) {
          if (OutstandingRequests[i] != null) {
            return OutstandingRequests[i];
          }
        }
      }

      return null;
    }

    public void FulfillRequest(ResourceRequest request) {
      OutstandingRequests[request.RequestingMarketStandIndex] = null;
      _score += (int)request.TimeRemainingSecs;
      // BIG TODO make this mean something
    }
  }
}
