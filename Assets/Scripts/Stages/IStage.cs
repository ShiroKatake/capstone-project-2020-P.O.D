using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract base interface for game stages.
/// </summary>
public interface IStage
{
    //Public Properties------------------------------------------------------------------------------------------------------------------------------

    //Basic Public Properties----------------------------------------------------------------------                                                                                                                          

    /// <summary>
    /// The EStage value denoting which stage this is
    /// </summary>
    EStage GetID();

    //Triggered Methods------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The main behaviour of the stage. 
    /// </summary>
    /// <note>
    /// If the stage follows a linear path, use while(waiting){yield return null} statements to delay behaviour. If the stage can loop back on itself or
    /// jump ahead, use an initial yield return null followed by while(step > -1){switch(step){/*stage content*/}.
    /// </note>
    IEnumerator Execution();
}
