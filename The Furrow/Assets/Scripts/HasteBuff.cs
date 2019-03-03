public class HasteBuff : Buff
{
    private int _factor;

    public HasteBuff(int duration, int factor)
    {
        Duration = duration;
        _factor = factor;
    }

    public int Duration { get; set; }
    public void Apply(Unit unit)
    {
        unit.TotalActionPoints += _factor;
        unit.ActionPoints += _factor;
    }

    public void Undo(Unit unit)
    {
        unit.TotalActionPoints -= _factor;
        unit.ActionPoints -= _factor;
    }

    public Buff Clone()
    {
        return new HasteBuff(Duration,_factor);
    }
}