# Pixels for Glory Extensions
Extensions used by Pixels for Glory libraries 

[![Build status](https://ci.appveyor.com/api/projects/status/7lwdnqh8b6nk37uv/branch/master?svg=true)](https://ci.appveyor.com/project/LlamaBot/extensions/branch/master)

## Building
Nothing special.  Build from solution.

## Installation
From a build or downloaded release, copy the `PixelsForGlory.ComputationalSystem.LSystem.dll` to `[PROJECT DIR]\Assets\Plugins`.

If using the Pixels for Glory NuGet repository at http://pixelsforglory.azurewebsites.net/nuget, install the `PixelsForGlory.LSystem` package into your own class library project or install the `PixelsForGlory.Unity3D.LSystem` package into a Unity3D project.

## Usage

### Context Sensitive Example

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
            var leftContext = currentNode.Previous.Value.NodeModule;
            var predecessor = currentNode.Value.NodeModule;
            var rightContext = currentNode.Next.Value.NodeModule;

            var node = 
                new LSystemNode<int>(stepNumber, new ContextSensitiveModuleB(leftContext.Data + rightContext.Data),
                new LinkedList<LSystemNode<int>>(new[] { new LSystemNode<int>(stepNumber, new ContextSensitiveModuleA(predecessor.Data)) }));

            return new List<LSystemNode<int>>(new[] { node });
        }
    }
    
    // In code somwhere...
    
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
    contextSensitiveLSystem.GetCurrentDerivation()));
    
### Parametric Example
 
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
    
    // Somewhere in code
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
        parametricLSystem.GetCurrentDerivation(intialState);
 
## Citations
Przemyslaw Prusinkiewicz, Mark James, and Radomír Měch. 1994. Synthetic topiary. In Proceedings of the 21st annual conference on Computer graphics and interactive techniques (SIGGRAPH '94). ACM, New York, NY, USA, 351-358. 
