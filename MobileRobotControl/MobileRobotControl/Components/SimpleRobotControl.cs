using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileRobotControl.Components
{
    class SimpleRobotControl : IRobotControl
    {

        public EnginesFill EnginesFill
        {
            get { return GetEnginesFill(); }
        }

        public bool aUp, aDown, aLeft, aRight;

        private EnginesFill GetEnginesFill()
        {
            if (aUp)
            {
                if (aLeft)
                {
                    return new EnginesFill()
                    {
                        LeftEngineFill = 30,
                        RightEngineFill = 100
                    };
                }
                if(aRight)
                {
                    return new EnginesFill()
                    {
                        LeftEngineFill = 100,
                        RightEngineFill = 30
                    };
                }

                return new EnginesFill()
                {
                    LeftEngineFill = 100,
                    RightEngineFill = 100
                };
            }

            if (aDown)
            {
                if (aLeft)
                {
                    return new EnginesFill()
                    {
                        LeftEngineFill = -30,
                        RightEngineFill = -90
                    };
                }
                if (aRight)
                {
                    return new EnginesFill()
                    {
                        LeftEngineFill = -90,
                        RightEngineFill = -30
                    };
                }

                return new EnginesFill()
                {
                    LeftEngineFill = -90,
                    RightEngineFill = -90
                };
            }

            if(aLeft)
                return new EnginesFill()
                {
                    LeftEngineFill = -60,
                    RightEngineFill = 60
                };

            if (aRight)
                return new EnginesFill()
                {
                    LeftEngineFill = 60,
                    RightEngineFill = -60
                };
            return new EnginesFill()
            {
                LeftEngineFill = 0,
                RightEngineFill = 0
            };
        }

    }
}
