public class DamageBuff : Buff
{
    private int _factor;

    public DamageBuff(int duration, int factor)
    {
        Duration = duration;
        _factor = factor;
    }

    public int Duration { get; set; }

    public void Apply(Unit unit)
    {

    }

    public void Undo(Unit unit)
    {
        
    }

    public void ApplySkillBuff(Skill skill)
    {
        skill.damage += _factor;
    }

    public void UndoSkillBuff(Skill skill)
    {
        skill.damage -= _factor;
    }



    public Buff Clone()
    {
        return new DamageBuff(Duration,_factor);
    }
}