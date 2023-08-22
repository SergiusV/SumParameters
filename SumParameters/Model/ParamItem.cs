using Autodesk.Revit.DB;

namespace SumParameters.Model
{
    public class ParamItem
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private bool _isParamType;
        public bool IsParamType
        {
            get { return _isParamType; }
            set { _isParamType = value; }
        }


        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
       
        public ParamItem (Parameter parameter)
        {
            Name = parameter.Definition.Name;
            switch (parameter.StorageType)
            {
                case StorageType.Integer:
                    Value = parameter.AsInteger();
                    break;
                case StorageType.Double:
                    Value = parameter.AsDouble();
                    try
                    {
                        if (parameter.GetUnitTypeId() == UnitTypeId.Millimeters)
                        {
                            Value = UnitUtils.ConvertFromInternalUnits(Value, UnitTypeId.Millimeters);
                            Unit = "мм";
                        }
                        else if (parameter.GetUnitTypeId() == UnitTypeId.Meters)
                        {
                            Value = UnitUtils.ConvertFromInternalUnits(Value, UnitTypeId.Meters);
                            Unit = "м";
                        }
                        else if (parameter.GetUnitTypeId() == UnitTypeId.CubicMeters)
                        {
                            Value = UnitUtils.ConvertFromInternalUnits(Value, UnitTypeId.CubicMeters);
                            Unit = "м3";
                        }
                        else if (parameter.GetUnitTypeId() == UnitTypeId.SquareMeters)
                        {
                            Value = UnitUtils.ConvertFromInternalUnits(Value, UnitTypeId.SquareMeters);
                            Unit = "м2";
                        }
                    }
                    catch {}
                    break;
                default:
                    break;
            }
        }
    }
}
