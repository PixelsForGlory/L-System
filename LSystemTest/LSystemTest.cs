// Copyright 2016 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PixelsForGlory.ComputationalSystem;

namespace LSystemTest
{
    #region ContextSensitive
    public class ContextSensitiveModuleA : LSystemModule<int>
    {
        public ContextSensitiveModuleA(int data) : base(data){}

        public override void ChangeState(LSystemState<int> systemState)
        {
            // Does nothing
        }
    }

    public class ContextSensitiveModuleB : LSystemModule<int>
    {
        public ContextSensitiveModuleB(int data) : base(data){}

        public override void ChangeState(LSystemState<int> systemState)
        {
            // Does nothing
        }
    }


    public class ContextSensitiveProduction1 : LSystemProduction<int>
    {
        public ContextSensitiveProduction1() : base(null, typeof(ContextSensitiveModuleA), null){}
        protected override bool Condition(LinkedListNode<LSystemNode<int>> currentNode)
        {
            return true;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<int>> currentNode)
        {
            return LSystemTest.RandomNumberGenerator.NextDouble() <= 0.4f;
        }

        protected override List<LSystemNode<int>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<int>> currentNode)
        {
            var a = currentNode.Value.NodeModule;
            var node = new LSystemNode<int>(stepNumber, new ContextSensitiveModuleA(a.Data + 1));
            return new List<LSystemNode<int>>(new[] { node });
        }
    }

    public class ContextSensitiveProduction2 : LSystemProduction<int>
    {
        public ContextSensitiveProduction2() : base(null, typeof(ContextSensitiveModuleA), null) { }

        protected override bool Condition(LinkedListNode<LSystemNode<int>> currentNode)
        {
            return true;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<int>> currentNode)
        {
            return LSystemTest.RandomNumberGenerator.NextDouble() <= 0.6f;
        }

        protected override List<LSystemNode<int>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<int>> currentNode)
        {
            var a = currentNode.Value.NodeModule;
            var node = new LSystemNode<int>(stepNumber, new ContextSensitiveModuleB(a.Data - 1));
            return new List<LSystemNode<int>>(new[] { node });
        }
    }

    public class ContextSensitiveProduction3 : LSystemProduction<int>
    {
        public ContextSensitiveProduction3() : base(typeof(ContextSensitiveModuleA), typeof(ContextSensitiveModuleB), typeof(ContextSensitiveModuleA)) { }

        protected override bool Condition(LinkedListNode<LSystemNode<int>> currentNode)
        {
            var b = currentNode.Value.NodeModule;
            return b.Data < 4;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<int>> currentNode)
        {
            return true;
        }

        protected override List<LSystemNode<int>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<int>> currentNode)
        {
            // ReSharper disable PossibleNullReferenceException
            var leftContext = currentNode.Previous.Value.NodeModule;
            var predecessor = currentNode.Value.NodeModule;
            var rightContext = currentNode.Next.Value.NodeModule;
            // ReSharper restore PossibleNullReferenceException

            var node = 
                new LSystemNode<int>(stepNumber, new ContextSensitiveModuleB(leftContext.Data + rightContext.Data),
                new LinkedList<LSystemNode<int>>(new[] { new LSystemNode<int>(stepNumber, new ContextSensitiveModuleA(predecessor.Data)) }));

            return new List<LSystemNode<int>>(new[] { node });
        }
    }
    #endregion

    public class ParametricModuleData : ICopy<ParametricModuleData>
    {
        public int PositionX;
        public int PositionY;
        public int StepsForward;
        public int Rotation;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", PositionX, PositionY, StepsForward, Rotation);
        }

        public ParametricModuleData ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public ParametricModuleData DeepCopy()
        {
            var returnData = new ParametricModuleData
            {
                PositionX = PositionX,
                PositionY = PositionY,
                StepsForward = StepsForward,
                Rotation = Rotation
            };

            return returnData;
        }
    }

    public class ParametricModuleEndPoint : LSystemModule<ParametricModuleData>
    {
        public ParametricModuleEndPoint(ParametricModuleData data) : base(data){}

        public override void ChangeState(LSystemState<ParametricModuleData> systemState)
        {
            // Do nothing
        }
    }

    public class ParametricModuleMoveForward : LSystemModule<ParametricModuleData>
    {
        public ParametricModuleMoveForward(ParametricModuleData data) : base(data){}

        public override void ChangeState(LSystemState<ParametricModuleData> systemState)
        {
            switch(systemState.CurrentState.Rotation)
            {
                case 0:
                    systemState.CurrentState.PositionY += Data.StepsForward;
                    break;
                case 90:
                    systemState.CurrentState.PositionX += Data.StepsForward;
                    break;
                case 180:
                    systemState.CurrentState.PositionY -= Data.StepsForward;
                    break;
                case 270:
                    systemState.CurrentState.PositionX -= Data.StepsForward;
                    break;
            }
        }
    }

    public class ParametricModuleRotate : LSystemModule<ParametricModuleData>
    {
        public ParametricModuleRotate(ParametricModuleData data) : base(data){}

        public override void ChangeState(LSystemState<ParametricModuleData> systemState)
        {
            systemState.CurrentState.Rotation += Data.Rotation;

            if(systemState.CurrentState.Rotation >= 360)
            {
                systemState.CurrentState.Rotation = 0;
            }
        }
    }

    public class ParametricModulePoint : LSystemQueryModule<ParametricModuleData>
    {
        public ParametricModulePoint(ParametricModuleData data) : base(data){}

        public override void ChangeState(LSystemState<ParametricModuleData> systemState)
        {
            // Do nothing
        }

        public override void QueryState(LSystemState<ParametricModuleData> systemState)
        {
            Data.PositionX = systemState.CurrentState.PositionX;
            Data.PositionY = systemState.CurrentState.PositionY;
        }
    }

    public class ParametricProdction1 : LSystemProduction<ParametricModuleData>
    {
        public ParametricProdction1() : base(null, typeof(ParametricModuleEndPoint), null){}

        protected override bool Condition(LinkedListNode<LSystemNode<ParametricModuleData>> predecessor)
        {
            return true;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<ParametricModuleData>> predecessor)
        {
            return true;
        }

        protected override List<LSystemNode<ParametricModuleData>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<ParametricModuleData>> predecessor)
        {
            var forwardModuleData =
                new ParametricModuleData()
                {
                    PositionX = 0,
                    PositionY = 0,
                    Rotation = 0,
                    StepsForward = 1
                };

            var rotateModuleData =
                new ParametricModuleData()
                {
                    PositionX = 0,
                    PositionY = 0,
                    Rotation = 90,
                    StepsForward = 0
                };

            var forwardNode = new LSystemNode<ParametricModuleData>(stepNumber, new ParametricModuleMoveForward(forwardModuleData));
            var pointNode = new LSystemNode<ParametricModuleData>(stepNumber, new ParametricModulePoint(new ParametricModuleData()));
            var rotationNode = new LSystemNode<ParametricModuleData>(stepNumber, new ParametricModuleRotate(rotateModuleData));
            var endPointNode = new LSystemNode<ParametricModuleData>(stepNumber, new ParametricModuleEndPoint(new ParametricModuleData()));

            return new List<LSystemNode<ParametricModuleData>>(new[] { forwardNode, pointNode, rotationNode, endPointNode });
        }
    }

    public class ParametricProduction2 : LSystemProduction<ParametricModuleData>
    {
        public ParametricProduction2() : base(null, typeof(ParametricModuleMoveForward), null){}

        protected override bool Condition(LinkedListNode<LSystemNode<ParametricModuleData>> predecessor)
        {
            return true;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<ParametricModuleData>> predecessor)
        {
            return true;
        }

        protected override List<LSystemNode<ParametricModuleData>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<ParametricModuleData>> predecessor)
        {
            var forwardModuleData =
                new ParametricModuleData()
                {
                    PositionX = 0,
                    PositionY = 0,
                    Rotation = 0,
                    StepsForward = predecessor.Value.NodeModule.Data.StepsForward + 1
                };

            var forwardNode = new LSystemNode<ParametricModuleData>(stepNumber, new ParametricModuleMoveForward(forwardModuleData));
            
            return new List<LSystemNode<ParametricModuleData>>(new[] { forwardNode });
        }
    }
    
    [TestClass]
    public class LSystemTest
    {
        public static Random RandomNumberGenerator;
        
        [TestMethod]
        public void ContextSensitiveTest()
        {
            RandomNumberGenerator = new Random(6);

            var axiom =
                new List<LSystemNode<int>>()
                {
                    new LSystemNode<int>(0, new ContextSensitiveModuleA(1)),
                    new LSystemNode<int>(0,new ContextSensitiveModuleB(2)),
                    new LSystemNode<int>(0,new ContextSensitiveModuleA(3))
                };

            var productions =
                new List<LSystemProduction<int>>
                {
                    new ContextSensitiveProduction1(),
                    new ContextSensitiveProduction2(),
                    new ContextSensitiveProduction3()
                };

            var contextSensitiveLSystem = new LSystem<int>(axiom, productions);

            contextSensitiveLSystem.RunProduction();

            Assert.AreEqual("A(1)B(4)[A(2)]A(3)", GetLSystemString(contextSensitiveLSystem.GetCurrentDerivation()));
        }

        private static string GetLSystemString<T>(LinkedList<LSystemNode<T>> lsystem)
        {
            string returnString = string.Empty;

            var currentNode = lsystem.First;
            while(currentNode != null)
            {
                string typeName = currentNode.Value.NodeModule.GetType().ToString();
                switch(typeName)
                {
                    case "LSystemTest.ContextSensitiveModuleA":
                        typeName = "A";
                        break;
                    case "LSystemTest.ContextSensitiveModuleB":
                        typeName = "B";
                        break;
                    case "LSystemTest.ParametricModuleMoveForward":
                        typeName = "F";
                        break;
                    case "LSystemTest.ParametricModulePoint":
                        typeName = "?P";
                        break;
                    case "LSystemTest.ParametricModuleRotate":
                        typeName = "-";
                        break;
                    case "LSystemTest.ParametricModuleEndPoint":
                        typeName = "A";
                        break;
                }

                string dataString = currentNode.Value.NodeModule.Data.ToString();

                if(currentNode.Value.NodeModule.Data.GetType().ToString() == "LSystemTest.ParametricModuleData")
                {
                    var stringParts = dataString.Split(',');

                    switch(typeName)
                    {
                        case "?P":
                            dataString = stringParts[0] + ", " + stringParts[1];
                            break;
                        case "F":
                            dataString = stringParts[2];
                            break;
                        default:
                            dataString = string.Empty;
                            break;
                    }
                }

                if(dataString != string.Empty)
                {
                    dataString = "(" + dataString + ")";
                }

                if(currentNode.Value.NodeModule != null)
                {
                    returnString += string.Format("{0}{1}", typeName, dataString);
                }

                if(currentNode.Value.SupportingBranch != null)
                {
                    returnString += string.Format("[{0}]", GetLSystemString(currentNode.Value.SupportingBranch));
                }

                currentNode = currentNode.Next;
            }

            return returnString;
        }

        [TestMethod]
        public void ParametricTest()
        {
            var axiom =
                new List<LSystemNode<ParametricModuleData>>()
                {
                    new LSystemNode<ParametricModuleData>(0, new ParametricModuleEndPoint(new ParametricModuleData()))
                };

            var productions =
                new List<LSystemProduction<ParametricModuleData>>
                {
                    new ParametricProdction1(),
                    new ParametricProduction2()
                };

            var parametricLSystem = new LSystem<ParametricModuleData>(axiom, productions);

            var intialState =
                new LSystemState<ParametricModuleData>(
                    new ParametricModuleData()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));

            parametricLSystem.RunProduction();

            intialState =
                new LSystemState<ParametricModuleData>(
                    new ParametricModuleData()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("F(1)?P(0, 1)-A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));

            parametricLSystem.RunProduction();

            intialState =
                new LSystemState<ParametricModuleData>(
                    new ParametricModuleData()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("F(2)?P(0, 2)-F(1)?P(1, 2)-A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));

            parametricLSystem.RunProduction();

            intialState =
                new LSystemState<ParametricModuleData>(
                    new ParametricModuleData()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("F(3)?P(0, 3)-F(2)?P(2, 3)-F(1)?P(2, 2)-A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));
        }
    }
}
