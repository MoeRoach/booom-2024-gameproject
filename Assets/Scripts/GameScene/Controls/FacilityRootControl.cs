// File create date:2024/4/19
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public class FacilityRootControl : MonoSingleton<FacilityRootControl> {

	private Dictionary<int, BaseFacilityControl> _facilityControls;
	private int _facilityIndex;

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
}