using System.Collections.Generic;
using static Agent;

public class HybridDecider : Decider
{
    private ReactiveModule reactiveModule;
    private BDIModule bDIModule;

    // 1� Reactive
    // Se houver a��o urgente, faz essa a��o
    // Sen�o vai ao BDI
    // Se houver um plano j� feito, faz a pr�xima a��o do plano
    // Sen�o tenta fazer um novo plano

    private void Awake()
    {
        reactiveModule = new ReactiveModule(this);
        bDIModule = new BDIModule(this);
    }

    public override void Decide(Perception perception)
    {

    }

    public override string GetArchitectureName()
    {
        return "Hybrid";
    }
}
