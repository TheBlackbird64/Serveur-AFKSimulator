

class Map
{
    public int valRnd { get; set; }

    public Map(int graine)
    {
        valRnd = graine;
        GenRnd();
    }

    public int GenRnd()
    {
        valRnd = (int)(16807 * valRnd % Math.Pow(2, 32));
        return valRnd;
    }


}