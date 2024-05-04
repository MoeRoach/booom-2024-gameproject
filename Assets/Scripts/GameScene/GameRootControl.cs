﻿// File create date:2024/4/19
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using RoachFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// Created By Yu.Liu
public class GameRootControl : MonoSingleton<GameRootControl> {

	private Tilemap _background;
	private Tilemap _terrain;
	private Tilemap _structure;

	private Transform _cameraPoint;
	private bool isDragging;
	private Vector3 startPointerPosition;
	private Vector3 realPointerPosition;

	private GameDirector _gameDirector;
	private GameDataService _gameData;

	private CountdownTimer _zoomTimer;

	protected override void OnAwake() {
		base.OnAwake();
		_background = FindComponent<Tilemap>("MapRoot.Background");
		_terrain = FindComponent<Tilemap>("MapRoot.Terrain");
		_structure = FindComponent<Tilemap>("MapRoot.Structure");
		_cameraPoint = FindComponent<Transform>("CameraPoint");
		_zoomTimer = new CountdownTimer(0.5f, true);
		_gameDirector = new GameDirector(this);
	}

	protected override void OnStart() {
		base.OnStart();
		_gameDirector.Init();
		_gameData = GameSystem.Instance.GameData;
		var sightArea = new Square(61, 34);
		var sightPivot = new Square(9 ,5);
		GameCameraManager.Instance.SetupSightLimit(sightPivot, sightArea);
		AsyncTask("InitStage", AsyncInitStage);
		RegisterUpdateFunction(1, UpdateCamera);
		RegisterUpdateFunction(1, _gameDirector.UpdateDirector);
	}

	protected override void SetupBroadcastFilter(BroadcastFilter filter) {
		_gameDirector.SetupBroadcastFilter(filter);
	}

	private void UpdateCamera() {
		if (Mouse.current.rightButton.wasPressedThisFrame) {
			isDragging = true;
			var mpos = Mouse.current.position.ReadValue();
			startPointerPosition = GameCameraManager.Instance.MainCamera.ScreenToWorldPoint(mpos);
			startPointerPosition.z = 0f;
		} else if (Mouse.current.rightButton.isPressed && isDragging) {
			var mpos = Mouse.current.position.ReadValue();
			realPointerPosition = GameCameraManager.Instance.MainCamera.ScreenToWorldPoint(mpos);
			realPointerPosition.z = 0f;
			var translateDir = realPointerPosition - startPointerPosition;
			var cameraPosition = _cameraPoint.position - translateDir;
			_cameraPoint.position = GameCameraManager.Instance.ClampCameraPointPosition(cameraPosition);
		} else if (Mouse.current.rightButton.wasReleasedThisFrame) {
			isDragging = false;
		}
		
		_zoomTimer.UpdateTimer(Time.unscaledDeltaTime);
		var scrollDis = Mouse.current.scroll.ReadValue().y / 64f;
		var isOverCanvas = EventSystem.current.IsPointerOverGameObject();
		if (Mathf.Approximately(scrollDis, 0f) || isOverCanvas) return;
		GameCameraManager.Instance.ChangeZoomLevel(scrollDis > 0f ? -1 : 1);
	}

	private async UniTask AsyncInitStage(CancellationTokenSource cts) {
		await UniTask.NextFrame(cts.Token);
		// 先构造关卡数据
		GenerateStageData();
		await UniTask.Yield(cts.Token);
		GeneratePawns();
		// 根据数据布局整个地图，包括物体的生成
		_gameData.stageGenerated = true;
	}

	private void GenerateStageData() {
		for (var x = -23; x <= 42; x++) {
			for (var y = 5; y <= 8; y++) {
				var sq = new Square(x, y);
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}

		for (var x = -15; x <= -7; x++) {
			for (var y = 13; y <= 20; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = -15; x <= -7; x++) {
			for (var y = -7; y <= 0; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = -12; x <= -10; x++) {
			for (var y = 1; y <= 12; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = 0; x <= 17; x++) {
			for (var y = 0; y <= 15; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = 5; x <= 12; x++) {
			for (var y = -13; y <= -1; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = 24; x <= 32; x++) {
			for (var y = 13; y <= 20; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = 24; x <= 32; x++) {
			for (var y = -7; y <= 0; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
		
		for (var x = 27; x <= 29; x++) {
			for (var y = 1; y <= 12; y++) {
				var sq = new Square(x, y);
				if (_gameData.stageData.tiles.ContainsKey(sq.Sid)) continue;
				_gameData.stageData.tiles[sq.Sid] = new SquareTile(x, y);
				_gameData.stageData.tileList.Add(sq);
			}
		}
	}

	private void GeneratePawns() {
		for (var i = 0; i < 5; i++) {
			var coord = _gameData.stageData.RandomTileCoord();
			PawnRootControl.Instance.SpawnPawn(PrefabUtils.PrefabPawnWorker, coord, Square.One);
		}
	}

	public override void ReceiveBroadcast(BroadcastInfo info) {
		_gameDirector.ReceiveBroadcast(info);
	}
}