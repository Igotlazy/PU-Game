using System.Collections.Generic;

public class SelectorPacket {

    public SelectorData selectorData;

    public struct Targets
    {
        public List<TargetSpecs> primaryTargets;
        public List<TargetSpecs> pecondaryTargets;
    }

    public Targets foundTargets = new Targets();

    private HashSet<Node> targetNodes = new HashSet<Node>();
    public HashSet<Node> TargetNodes
    {
        get
        {
            return targetNodes;
        }
        set
        {
            targetNodes = value;
        }
    }

    public List<TargetSpecs> targetObjectSpecs = new List<TargetSpecs>();

}
