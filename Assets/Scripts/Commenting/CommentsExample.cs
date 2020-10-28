using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An example class for how to go about grouping fields, properties and methods using comments.
/// Note: does not demonstrate triple-slash summary comments for properties and methods, but use those too.
/// See CommentsTemplate for copy-and-paste examples. If CommentsExample clashes with CommentsTemplate,
/// assume CommentsTemplate is correct.
/// </summary>
public class CommentsExample : MonoBehaviour                                                                                                           //If the class needs to be singleton, it should inherit from SerializableSingleton
                                                                                                                                                       //if it needs to be a MonoBehaviour as well, or from Singleton if it should be
                                                                                                                                                       //instantiated only when it's called in code.
{
    //Note: the comments here are just the organisational ones separating methods and fields and whatnot.                                              //You don't need to put all your comments out here. I just put the
    //For descriptive / explanatory comments, put them above or to the right of code as you see fit and                                                //explanatory ones out here because it was clear and made sense.
    //as is sensible to do.

    //Note: for properties and methods, add triple-slash comments for method and property descriptions and
    //parameter descriptions so that you can see them when you hover over a property or method.

    //Private Fields---------------------------------------------------------------------------------------------------------------------------------  //Dashes extend out to column 150; note the space separating this line and the next

    //Serialized Fields----------------------------------------------------------------------------                                                    //Fields exposed in the inspector to us devs, but not to other classes. Dashes extend
                                                                                                                                                       //out to column 100 for subsections of larger 150-dash sections
    [Header("Name of Logical Group")]
    [SerializeField] private string someSerializedVariable;

    [Header("Name of Another Logical Group")]
    [SerializeField] private string someOtherSerializedVariable;

    //Non-Serialized Fields------------------------------------------------------------------------                                                    //Not visible to devs or other classes

    //Some logical group                                                                                                                               //More granular grouping comments like this can just go on the line above the group 
    private string someNonSerializedVariable;

    //Some other logical group
    private string someOtherNonSerializedVariable;

    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          //Public properties that are just get and/or set properties; listed in alphabetical order

    public string SomeOtherSerializedVariable { get => someOtherSerializedVariable; set => someOtherSerializedVariable = value; }
    public string SomeSerializedVariable { get => someSerializedVariable; }

    //Complex Public Properties--------------------------------------------------------------------                                                    //Public properties with more than just get/set logic; listed in alphabetical order

    public string SomeNonSerializedVariable
    {
        get
        {
            return someNonSerializedVariable;
        }

        set
        {
            //Explanatory comment
            someNonSerializedVariable = (value == "") ? "[Empty, for example]" : value;
        }
    }

    public string SomeOtherNonSerializedVariable
    {
        get
        {
            return someOtherNonSerializedVariable;
        }

        set
        {
            //Explanatory comment
            switch(someOtherNonSerializedVariable.Length)
            {
                case 0:
                    someOtherNonSerializedVariable = "[Blank]";
                    break;
                case 1:
                case 2:
                case 3:
                    someOtherNonSerializedVariable = value;
                    break;
                default:
                    someOtherNonSerializedVariable = $"Some other redundant value that also includes the passed value, which is: {value}";
                    break;
            }
        }
    }

    //Initialization Methods-------------------------------------------------------------------------------------------------------------------------

    //Method summary comment
    private void Awake()
    {

    }

    //Method summary comment
    private void Start()
    {
        //Explanatory grouping comment
        DoInitializationThingOnStart();
        DoOtherInitializationThingOnStart();

        //Explanatory grouping comment with line between it and the last group
        InvokeRepeating(nameof(InvokedRecurringThingOrMaybeACoroutineButCoroutinesUseDifferentSyntaxButActuallyDontMakeItsNameThisLongThisIsNotAFeasiblyLengthedVariableOrMethodName), 1f, 1f);
    }

    //Method summary comment
    private void DoInitializationThingOnStart()
    {
        //Stuff
    }

    //Method summary comment
    private void DoOtherInitializationThingOnStart()
    {
        //Other stuff
    }

    //Core Recurring Methods-------------------------------------------------------------------------------------------------------------------------  //Update(), FixedUpdate(), any major co-routines or methods called by InvokeRepeating().

    //Method summary comment
    private void Update()
    {
        //Explanatory grouping comment
        RegularThingA();
        RegularThingB();

        //Explanatory grouping comment
        if (true)
        {
            ConditionalRegularThing();
        }
    }

    //Method summary comment
    private void FixedUpdate()
    {
        //Explanatory grouping comment
        FixedRegularThingA();

        //Explanatory grouping comment
        FixedRegularThingB();

        if (true)
        {
            FixedConditionalRegularThing();
        }
    }

    //Method summary comment
    private void InvokedRecurringThingOrMaybeACoroutineButCoroutinesUseDifferentSyntaxButActuallyDontMakeItsNameThisLongThisIsNotAFeasiblyLengthedVariableOrMethodName()
    {
        //Explanatory grouping comment
        RecurringThingOne();

        //Explanatory grouping comment
        RecurringThingTwo();

        if (!false)
        {
            ConditionalRecurringThing();
        }
    }

    //Recurring Methods (Update())-------------------------------------------------------------------------------------------------------------------  //Methods used by Update()

    //Method summary comment
    private void RegularThingA()
    {
        //Stuff
    }

    //Some Logical Subgroup------------------------------------------------------------------------                                                    //Dashes extend to column 100. If you've got logical subgroups of methods you want to split  
                                                                                                                                                       //out and highlight, go nuts (but not overboard; go nuts sensibly). If they fit under a  
    //Method summary comment                                                                                                                           //particular section, make it a subgroup of that section.
    private void RegularThingB()
    {
        //Stuff

        if (StringsAreIdentical("Uhh", "Whatever"))
        {
            //More stuff
        }
    }

    //Method summary comment
    private void ConditionalRegularThing()
    {
        //Stuff
    }

    //Recurring Methods (FixedUpdate())--------------------------------------------------------------------------------------------------------------  //Methods used by FixedUpdate() (which runs at a fixed rate independent of framerate, used for  
                                                                                                                                                       //physics stuff)
    //Method summary comment
    private void FixedRegularThingA()
    {
        //Stuff
    }

    //Method summary comment
    private void FixedRegularThingB()
    {
        //Stuff

        if (StringsAreIdentical("Blah", "Ppppppppppppppppppppppppphhhhhhhhhhhhhhhhhhhhhhyyyyyyyyyyyyyyyyyyyyyyssssssssssssssssssssssiiiiiiiiiiiiiiiiiiiiiiiiccccccccccccccccccccccssssssssssssssssssssssssss yea"))
        {
            //More stuff
        }
    }

    //Method summary comment
    private void FixedConditionalRegularThing()
    {
        //Stuff
    }

    //Recurring Methods (Other)----------------------------------------------------------------------------------------------------------------------  //Methods co-routines or InvokeRepeating() methods, which get used at different intervals to
                                                                                                                                                       //Update() (every frame) and FixedUpdate() (every nth fraction of a second)
    //Method summary comment
    private void RecurringThingOne()
    {
        //Stuff
    }

    //Method summary comment
    private void RecurringThingTwo()
    {
        //Stuff

        if (StringsAreIdentical("Uhh", "Whatever"))
        {
            //More stuff
        }
    }

    //Method summary comment
    private void ConditionalRecurringThing()
    {
        //Stuff
    }

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------  //Methods that get called at specific times due to specific actions or something

    //Method summary comment
    public void DoSomething()
    {
        //Do "something"
    }

    //Method summary comment
    public void DoSomethingElse()
    {
        if (StringsAreIdentical("For example", "Something"))
        {
            //Do "something else"
        }
        else
        {
            //Get stuffed
        }
    }

    //Some Other Sensibly Named Group----------------------------------------------------------------------------------------------------------------  //If you've got logical groups of methods you want to split out and highlight, go nuts (but 
                                                                                                                                                       //not overboard; go nuts sensibly)
    //Method summary comment
    private void SomeOtherSensiblyNamedMethod()
    {
        //Sensible stuff
    }

    //Utility Methods--------------------------------------------------------------------------------------------------------------------------------  //Methods that are basically just tools and basic utilities, like comparing values, getting 
                                                                                                                                                       //distances, etc.
    //Method summary comment
    private bool StringsAreIdentical(string a, string b)
    {
        return a == b;
    }
}
