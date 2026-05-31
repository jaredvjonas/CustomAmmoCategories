using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using BattleTech;
using BattleTech.UI;

namespace CustAmmoCategories {
  // Minimal math helpers used by the codebase
  public static class NvMath {
    public static bool FloatIsNearZero(float v, float eps = 0.0001f) {
      return Math.Abs(v) <= eps;
    }
    public static float AngleSigned(Vector3 from, Vector3 to, Vector3 axis) {
      float angle = Vector3.Angle(from, to);
      float sign = Math.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
      return angle * sign;
    }
  }

  // Provide a small extension used by UI code to set the component ref on list items
  public static class LanceMechEquipmentListItemExtensions {
    public static void SetComponentRef(this LanceMechEquipmentListItem item, MechComponentRef componentRef, MechDef mechDef) {
      if (item == null) return;
      try {
        var t = item.GetType();
        // try set field 'componentRef'
        var f = t.GetField("componentRef", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (f != null) { f.SetValue(item, componentRef); }
        // try set property 'ComponentRef'
        var p = t.GetProperty("ComponentRef", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null && p.CanWrite) { p.SetValue(item, componentRef); }
        // try to refresh the UI if method exists
        var m = t.GetMethod("SetIconAndText", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (m != null) { m.Invoke(item, null); }
      } catch (Exception) { }
    }
    public static MechComponentRef GetComponentRef(this LanceMechEquipmentListItem item) {
      if (item == null) return null;
      try {
        var t = item.GetType();
        var f = t.GetField("componentRef", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (f != null) { return (MechComponentRef)f.GetValue(item); }
        var p = t.GetProperty("ComponentRef", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null && p.CanRead) { return (MechComponentRef)p.GetValue(item); }
      } catch (Exception) { }
      return null;
    }
  }
}
