using System;

public enum CombatPhase
{
    FreeMove,    // ← movimiento libre
    OnGuard,     // ← “freez” en nuestro manager (ventana de input/planeamiento)
    Performance  // ← ejecución
}

public interface ICombatPhaseSource
{
    CombatPhase Current { get; }
    event Action<CombatPhase> OnPhaseChanged;
}