﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame.Cards
{
  public class SearchlightEffect : EffectBase
  {

    public SearchlightEffect( Player player, Cabin target ) : base( player, target ) { }


    public override void ApplyEffect()
    {

      var targetPlayer = Target.Player;

      if ( targetPlayer == null )
      {
        Player.WriteMessage( "探测 {0} 号舱，发现 {1} 玩家。", Target.Index, targetPlayer.CodeName );
        targetPlayer.WriteMessage( "玩家 {0} 探测到了您的位置！", Player.CodeName );
      }
      else
        Player.WriteMessage( "探测 {0} 号舱，是一个空舱。", Target.Index );
    }
  }
}
