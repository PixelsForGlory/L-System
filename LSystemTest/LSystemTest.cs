// Copyright 2016 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PixelsForGlory.ComputationalSystem;

namespace LSystemTest
{
    #region ContextSensitive
    public class ContextSensitiveModuleA : ILSystemModule<int>
    {
        public int F { get; private set; }

        public ContextSensitiveModuleA(int f)
        {
            F = f;
        }

        public void ChangeState(LSystemState<int> systemState) { /* Does nothing */ }
    }

    public class ContextSensitiveModuleB : ILSystemModule<int>
    {
        public int F { get; private set; }

        public ContextSensitiveModuleB(int f)
        {
            F = f;
        }

        public void ChangeState(LSystemState<int> systemState) { /* Does nothing */ }
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
            var a = (ContextSensitiveModuleA)currentNode.Value.NodeModule;
            var node = new LSystemNode<int>(stepNumber, new ContextSensitiveModuleA(a.F + 1));
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
            var a = (ContextSensitiveModuleA)currentNode.Value.NodeModule;
            var node = new LSystemNode<int>(stepNumber, new ContextSensitiveModuleB(a.F - 1));
            return new List<LSystemNode<int>>(new[] { node });
        }
    }

    public class ContextSensitiveProduction3 : LSystemProduction<int>
    {
        public ContextSensitiveProduction3() : base(typeof(ContextSensitiveModuleA), typeof(ContextSensitiveModuleB), typeof(ContextSensitiveModuleA)) { }

        protected override bool Condition(LinkedListNode<LSystemNode<int>> currentNode)
        {
            var b = (ContextSensitiveModuleB)currentNode.Value.NodeModule;
            return b.F < 4;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<int>> currentNode)
        {
            return true;
        }

        protected override List<LSystemNode<int>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<int>> currentNode)
        {
            // ReSharper disable PossibleNullReferenceException
            var leftContext = (ContextSensitiveModuleA)currentNode.Previous.Value.NodeModule;
            var predecessor = (ContextSensitiveModuleB)currentNode.Value.NodeModule;
            var rightContext = (ContextSensitiveModuleA)currentNode.Next.Value.NodeModule;
            // ReSharper restore PossibleNullReferenceException

            var node = 
                new LSystemNode<int>(stepNumber, new ContextSensitiveModuleB(leftContext.F + rightContext.F),
                new LinkedList<LSystemNode<int>>(new[] { new LSystemNode<int>(stepNumber, new ContextSensitiveModuleA(predecessor.F)) }));

            return new List<LSystemNode<int>>(new[] { node });
        }
    }
    #endregion

    public class ParametricState : ICopy<ParametricState>
    {
        public int PositionX;
        public int PositionY;
        public int Rotation;

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", PositionX, PositionY, Rotation);
        }

        public ParametricState ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public ParametricState DeepCopy()
        {
            var returnData = new ParametricState
            {
                PositionX = PositionX,
                PositionY = PositionY,
                Rotation = Rotation
            };

            return returnData;
        }
    }

    public class ParametricModuleEndPoint : ILSystemModule<ParametricState>
    {
        public void ChangeState(LSystemState<ParametricState> systemState) { /* Do nothing */ }
    }

    public class ParametricModuleMoveForward : ILSystemModule<ParametricState>
    {
        public int StepsForward { get; private set; }

        public ParametricModuleMoveForward(int stepsForward)
        {
            StepsForward = stepsForward;
        }

        public void ChangeState(LSystemState<ParametricState> systemState)
        {
            switch(systemState.CurrentState.Rotation)
            {
                case 0:
                    systemState.CurrentState.PositionY += StepsForward;
                    break;
                case 90:
                    systemState.CurrentState.PositionX += StepsForward;
                    break;
                case 180:
                    systemState.CurrentState.PositionY -= StepsForward;
                    break;
                case 270:
                    systemState.CurrentState.PositionX -= StepsForward;
                    break;
            }
        }
    }

    public class ParametricModuleRotate : ILSystemModule<ParametricState>
    {
        public int Rotation { get; private set; }

        public ParametricModuleRotate(int rotation)
        {
            Rotation = rotation;
        }

        public void ChangeState(LSystemState<ParametricState> systemState)
        {
            systemState.CurrentState.Rotation += Rotation;

            if(systemState.CurrentState.Rotation >= 360)
            {
                systemState.CurrentState.Rotation = 0;
            }
        }
    }

    public class ParametricModulePoint : ILSystemQueryableModule<ParametricState>
    {
        public int QueriedPositionX { get; private set; }
        public int QueriedPositionY { get; private set; }

        public void ChangeState(LSystemState<ParametricState> systemState) { /* Does nothing */ }

        public void QueryState(LSystemState<ParametricState> systemState)
        {
            QueriedPositionX = systemState.CurrentState.PositionX;
            QueriedPositionY = systemState.CurrentState.PositionY;
        }
    }

    public class ParametricProdction1 : LSystemProduction<ParametricState>
    {
        public ParametricProdction1() : base(null, typeof(ParametricModuleEndPoint), null){}

        protected override bool Condition(LinkedListNode<LSystemNode<ParametricState>> predecessor)
        {
            return true;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<ParametricState>> predecessor)
        {
            return true;
        }

        protected override List<LSystemNode<ParametricState>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<ParametricState>> predecessor)
        {
            var forwardNode = new LSystemNode<ParametricState>(stepNumber, new ParametricModuleMoveForward(1));
            var pointNode = new LSystemNode<ParametricState>(stepNumber, new ParametricModulePoint());
            var rotationNode = new LSystemNode<ParametricState>(stepNumber, new ParametricModuleRotate(90));
            var endPointNode = new LSystemNode<ParametricState>(stepNumber, new ParametricModuleEndPoint());

            return new List<LSystemNode<ParametricState>>(new[] { forwardNode, pointNode, rotationNode, endPointNode });
        }
    }

    public class ParametricProduction2 : LSystemProduction<ParametricState>
    {
        public ParametricProduction2() : base(null, typeof(ParametricModuleMoveForward), null){}

        protected override bool Condition(LinkedListNode<LSystemNode<ParametricState>> predecessor)
        {
            return true;
        }

        protected override bool Probability(LinkedListNode<LSystemNode<ParametricState>> predecessor)
        {
            return true;
        }

        protected override List<LSystemNode<ParametricState>> CreateSuccessor(int stepNumber, LinkedListNode<LSystemNode<ParametricState>> predecessor)
        {
            var module = (ParametricModuleMoveForward) predecessor.Value.NodeModule;
            var forwardNode = new LSystemNode<ParametricState>(stepNumber, new ParametricModuleMoveForward(module.StepsForward + 1));
            
            return new List<LSystemNode<ParametricState>>(new[] { forwardNode });
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
                    new LSystemNode<int>(0, new ContextSensitiveModuleB(2)),
                    new LSystemNode<int>(0, new ContextSensitiveModuleA(3))
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

        [TestMethod]
        public void ParametricTest()
        {
            var axiom =
                new List<LSystemNode<ParametricState>>()
                {
                    new LSystemNode<ParametricState>(0, new ParametricModuleEndPoint())
                };

            var productions =
                new List<LSystemProduction<ParametricState>>
                {
                    new ParametricProdction1(),
                    new ParametricProduction2()
                };

            var parametricLSystem = new LSystem<ParametricState>(axiom, productions);

            var intialState =
                new LSystemState<ParametricState>(
                    new ParametricState()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));

            parametricLSystem.RunProduction();

            intialState =
                new LSystemState<ParametricState>(
                    new ParametricState()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("F(1)?P(0, 1)-A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));

            parametricLSystem.RunProduction();

            intialState =
                new LSystemState<ParametricState>(
                    new ParametricState()
                    {
                        PositionX = 0,
                        PositionY = 0
                    });
            Assert.AreEqual("F(2)?P(0, 2)-F(1)?P(1, 2)-A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));

            parametricLSystem.RunProduction();

            intialState =
                new LSystemState<ParametricState>(
                    new ParametricState()
                    {
                        PositionX = 0,
                        PositionY = 0,
                        Rotation = 0
                    });
            Assert.AreEqual("F(3)?P(0, 3)-F(2)?P(2, 3)-F(1)?P(2, 2)-A", GetLSystemString(parametricLSystem.GetCurrentDerivation(intialState)));
        }


        private static string GetLSystemString<T>(LinkedList<LSystemNode<T>> lsystem)
        {
            string returnString = string.Empty;

            var currentNode = lsystem.First;
            while(currentNode != null)
            {
                string typeName = currentNode.Value.NodeModule.GetType().ToString();
                string dataString = string.Empty;
                if(typeName == "LSystemTest.ContextSensitiveModuleA")
                {
                    typeName = "A";
                    var module = (ContextSensitiveModuleA)currentNode.Value.NodeModule;
                    dataString = module.F.ToString();
                }
                else if(typeName == "LSystemTest.ContextSensitiveModuleB")
                {
                    typeName = "B";
                    var module = (ContextSensitiveModuleB)currentNode.Value.NodeModule;
                    dataString = module.F.ToString();
                }
                else if(typeName == "LSystemTest.ParametricModuleMoveForward")
                {
                    typeName = "F";
                    var module = (ParametricModuleMoveForward)currentNode.Value.NodeModule;
                    dataString = module.StepsForward.ToString();
                }
                else if(typeName == "LSystemTest.ParametricModulePoint")
                {
                    typeName = "?P";
                    var module = (ParametricModulePoint)currentNode.Value.NodeModule;
                    dataString = module.QueriedPositionX + ", " + module.QueriedPositionY;
                }
                else if(typeName == "LSystemTest.ParametricModuleRotate")
                {
                    typeName = "-";
                }
                else if(typeName == "LSystemTest.ParametricModuleEndPoint")
                {
                    typeName = "A";
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
    }
}
