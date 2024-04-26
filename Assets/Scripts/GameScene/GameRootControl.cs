// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

// Created By Yu.Liu
public class GameRootControl : MonoSingleton<GameRootControl> {

	private Tilemap _background;
	private Tilemap _terrain;
	private Tilemap _structure;

	protected override void OnAwake() {
		base.OnAwake();
		_background = FindComponent<Tilemap>("MapRoot.Background");
		_terrain = FindComponent<Tilemap>("MapRoot.Terrain");
		_structure = FindComponent<Tilemap>("MapRoot.Structure");
	}
}