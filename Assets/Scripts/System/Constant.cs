public static class Constant
{
	public static readonly int DEFAULT_CAMERA_DISTANCE = 10;
	public static readonly int DEFAULT_CAMERA_HEIGHT = 5;

	public static readonly float DEFAULT_ACTOR_SPEED = 3.5f;
	public static readonly int TICK_RATE = 500;                 // Tick rate in milliseconds.
	public static readonly float SCALED_TICK_RATE = TICK_RATE / 1000f;	// Tick rate in seconds.
	public static readonly int BASE_INFLUENCE_RADIUS = 5;
	public static readonly int INFLUENCE_RADIUS_INCREMENT = 5;
	public static readonly int PLACEMENT_MAXIMUM_RADIUS = 100;
	public static readonly float DEFAULT_REFRESH_RATE = 1 / 60f;
	public static readonly int AGENT_MAX_SLOPE = 45;
	public static readonly float AGENT_BASE_WALK_SPEED = 1.5f;
	public static readonly float AGENT_BASE_RUN_SPEED = 4.5f;

	public static readonly float ACTOR_BASE_SPEED = 3.5f;
	public static readonly float ACTOR_BASE_ROTATION_SPEED = 480f;
	public static readonly float ACTOR_BASE_FOOD = 5f;

	public static readonly string STARVING = "Starving";
	public static readonly string HUNGRY = "Hungry";
	public static readonly string DEATH = "Dead";
	public static readonly string REFLECT = "reflect";
	public static readonly string STUNNED = "stunned";

	public static readonly float DEFAULT_TEMPERATURE = 20f;

	public static readonly int MINIMUM_LEVEL = 1;
	public static readonly int MAXIMUM_LEVEL = 100;

	public static readonly int MAXIMUM_LUCK = 100;

	public static readonly float TOOLTIP_DELAY = 0.2f;

	// The tick rate for the rifts (in seconds).
	public static readonly float RIFT_TICK_RATE = 10.0f;

	// Character Attributes
	public static readonly float DEFAULT_HEALTH = 100;
	public static readonly float DEFAULT_SPEED = 1.0f;
	public static readonly float DEFAULT_DAMAGE = 1.0f;
	public static readonly float DEFAULT_HEALTH_REGENERATION = 1.00f;
	public static readonly float DEFAULT_HUNGER_RATE = 0.05f;
	public static readonly float DEFAULT_FOOD = 5f;

	// Combat
	public static readonly float ARMOR_DAMAGE_REDUCTION_FACTOR = 0.5f; // Each point of armor reduces damage by 50%
	public static readonly float DAMAGE_JITTER_FACTOR = 0.2f; // Damage can vary by ±20%
}
