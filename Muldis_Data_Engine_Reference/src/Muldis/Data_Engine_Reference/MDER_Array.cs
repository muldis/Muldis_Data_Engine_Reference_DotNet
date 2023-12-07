namespace Muldis.Data_Engine_Reference;

public class MDER_Array : MDER_Any
{
    internal Internal_MDER_Array_Struct tree_root_node;

    internal MDER_Array(MDER_Machine machine, Internal_MDER_Array_Struct tree_root_node)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Array)
    {
        this.tree_root_node = tree_root_node;
    }

    internal String _as_MUON_Array_artifact(String indent)
    {
        String mei = indent + "\u0009";
        return Object.ReferenceEquals(this, this.machine().MDER_Array_C0) ? "(Array:[])"
            : "(Array:[\u000A" + this._as_MUON_Array_artifact__node__tree(
                this.tree_root_node, mei) + indent + "])";
    }

    private String _as_MUON_Array_artifact__node__tree(Internal_MDER_Array_Struct node, String indent)
    {
        // Note: We always display consecutive duplicates in repeating
        // value format rather than value-count format in order to keep
        // everything simple and reliable in the 99% common case; we
        // assume that it would only be a rare edge case to have a
        // sparse array that we would want to output in that manner.
        // TODO: Rendering a Singular with a higher multiplicity than
        // an Int32 can hold will throw a runtime exception; we can
        // deal with that later if an actual use case runs afowl of it.
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Array_Type.None:
                return "";
            case Internal_Symbolic_Array_Type.Singular:
                return String.Concat(Enumerable.Repeat(
                    indent + node.Local_Singular_Members().member._as_MUON_Any_artifact(indent) + ",\u000A",
                    (Int32)node.Local_Singular_Members().multiplicity));
            case Internal_Symbolic_Array_Type.Arrayed:
                return String.Concat(Enumerable.Select(
                    node.Local_Arrayed_Members(),
                    m => indent + m._as_MUON_Any_artifact(indent) + ",\u000A"));
            case Internal_Symbolic_Array_Type.Catenated:
                return this._as_MUON_Array_artifact__node__tree(node.Tree_Catenated_Members().a0, indent)
                    + this._as_MUON_Array_artifact__node__tree(node.Tree_Catenated_Members().a1, indent);
            default:
                throw new NotImplementedException();
        }
    }
}
