using System.Collections.Generic;
using static Agent;

public class HybridDecider : Decider
{
    private ReactiveModule reactiveModule;
    private BDIModule bDIModule;

    // 1º Reactive
    // Se houver ação urgente, faz essa ação
    // Senão vai ao BDI
    // Se houver um plano já feito, faz a próxima ação do plano
    // Senão tenta fazer um novo plano

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
