// File create date:2024/4/29
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public static class PrefabUtils {

	public const string PdbStagePawns = "StagePawns";

	public const string PrefabDefaultPawn = "DefaultPawn";
	public const string PrefabPawnWorker = "PawnWorker";

	public static GameObject CreateStagePawn(Transform root, string pn, int i = 0) {
		return PrefabManager.Instance.CreateFromPrefab(PdbStagePawns, pn, root, i);
	}

	public const string PdbStageObjects = "StageObjects";

	public const string PrefabDefaultObject = "DefaultObject";

	public static GameObject CreateStageObject(Transform root, string on, int i = 0) {
		return PrefabManager.Instance.CreateFromPrefab(PdbStageObjects, on, root, i);
	}

	public const string PdbStageFacilities = "StageFacilities";

	public const string PrefabDefaultFacility = "DefaultFacility";

	public static GameObject CreateStageFacility(Transform root, string fn, int i = 0) {
		return PrefabManager.Instance.CreateFromPrefab(PdbStageFacilities, fn, root, i);
	}
}