// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class FacilityRootControl : MonoSingleton<FacilityRootControl> {

	private Dictionary<int, BaseFacilityControl> _facilityControls;
	private int _facilityIndex;

	public BaseFacilityControl this[int id] => _facilityControls?.TryGetElement(id);

	private int FacilityIndex {
		get {
			_facilityIndex++;
			return _facilityIndex - 1;
		}
	}

	protected override void OnAwake() {
		base.OnAwake();
		_facilityControls = new Dictionary<int, BaseFacilityControl>();
	}
	
	public void SpawnFacility(string fn, Square coord, int i = 0) {
		var fo = PrefabUtils.CreateStageFacility(transform, fn, i);
		var ctrl = fo.GetComponent<BaseFacilityControl>();
		ctrl.RegisterDestroyCallback(OnFacilityDead);
		ctrl.SetupIdentifier(FacilityIndex);
		ctrl.SetupCoord(coord);
		ctrl.NotifyGenerate();
		_facilityControls[ctrl.Id] = ctrl;
	}

	private void OnFacilityDead(GameObject fo) {
		var ctrl = fo.GetComponent<BaseFacilityControl>();
		if (ctrl == null) return;
		_facilityControls.Remove(ctrl.Id);
	}
}