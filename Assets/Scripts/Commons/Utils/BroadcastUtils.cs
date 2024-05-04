// File create date:2024/5/2
using System;
using System.Collections.Generic;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public static class BroadcastUtils {
	
	public const string BroadcastFilterObjectNotify = "ObjectNotify";
	
	public const string BroadcastContentGenerated = "Generated";
	public const string BroadcastContentDestroyed = "Destroyed";

	public const string ExtraTagObjectType = "ObjectType";

	public static void NotifyObjectGenerated(string ot) {
		var info = BroadcastInfo.Create(BroadcastFilterObjectNotify, BroadcastContentGenerated);
		info.PutStringExtra(ExtraTagObjectType, ot);
		GameSystem.Instance.Broadcast.BroadcastInformation(info);
	}

	public static void NotifyObjectDestroyed(string ot) {
		var info = BroadcastInfo.Create(BroadcastFilterObjectNotify, BroadcastContentDestroyed);
		info.PutStringExtra(ExtraTagObjectType, ot);
		GameSystem.Instance.Broadcast.BroadcastInformation(info);
	}
}