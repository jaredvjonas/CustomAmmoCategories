using BattleTech;

namespace IRBTModUtils.Feature {
  public static class MovementFeatureShim {
    public static bool IsImmobile(AbstractActor actor) {
      if (actor == null) return false;
      // best-effort: check for a flag or property if available via reflection
      try {
        var t = actor.GetType();
        var f = t.GetField("m_isImmobile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (f != null) { return (bool)f.GetValue(actor); }
      } catch { }
      return false;
    }
    public static void SetImmobile(AbstractActor actor, bool immobile) {
      if (actor == null) return;
      try {
        var t = actor.GetType();
        var f = t.GetField("m_isImmobile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (f != null) { f.SetValue(actor, immobile); return; }
      } catch { }
    }
  }
}
