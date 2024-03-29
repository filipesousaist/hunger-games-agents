﻿/// <summary>
/// Global constants used by multiple classes.
/// </summary>
public static class Const 
{
    public const int NUM_AGENTS = 8;
    public const int NUM_REGIONS = 8;
    public const float DECISION_TIME = 0.025f;
    public const float SPAWN_RADIUS = 15;
    public const int WORLD_SIZE = 250;

    public const int MAX_ENERGY = 100;
    public const int MAX_ATTACK = 10;

    public const int HUNGER_PERIOD = 500;

    public const int TRAIN_DURATION = 100; // Epochs
    public const int TRAIN_ATTACK_GAIN = 1;
    public const int TRAIN_ENERGY_LOSS = 5;

    public const float WALK_DISTANCE = 0.25f; // Distance to walk in one epoch
    public const float ROTATE_ANGLE = 3;
    
    public const int BOW_MAX_ATTACK = 5 ;
    public const int SWORD_MAX_ATTACK = 8;
    
    public const int DANGEROUS_BOW_ANGLE = 15;
    public const int DANGEROUS_MELEE_DISTANCE = 5;
    
    public const int BOW_MIN_ANGLE = 2;
    
    public const int RADIUS_HAZARD_SPAWN = 50;

    public const int EPOCHS_TO_OPEN_CHEST = 40;
}
