// File create date:2024/5/2
using System;
using System.Collections.Generic;
using UnityEngine;
// Created By Yu.Liu
public class MonsterCorpse : BaseObjectControl  {
	
	protected override void OnObjectGenerated() {
		base.OnObjectGenerated();
		BroadcastUtils.NotifyObjectGenerated(ObjectConfigs.ObjectTypeMonsterCorpse);
	}

	protected override void Release() {
		BroadcastUtils.NotifyObjectDestroyed(ObjectConfigs.ObjectTypeMonsterCorpse);
		base.Release();
	}
}