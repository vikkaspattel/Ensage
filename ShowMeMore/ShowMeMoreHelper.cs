namespace  Overlay_information
{
    internal  class  ShowMeMoreHelper
    {
        public  string Modifier;
        public  string EffectName;
        public  string SecondeffectName;
        public  int Range;

        /// <Summary>
        /// 
        /// </ Summary>
        /// <Param  name = "modifier"> </ param>
        /// <Param  name = "effectName"> </ param>
        /// <Param  name = "secondeffectName"> </ param>
        /// <Param  name = "range"> </ param>
        public  ShowMeMoreHelper (string  modifier, string  effectName, string  secondeffectName, int  range)
        {
            Modifier = modifier;
            this .EffectName = effectName;
            SecondeffectName = secondeffectName;
            Range = range;
        }
    }
}
