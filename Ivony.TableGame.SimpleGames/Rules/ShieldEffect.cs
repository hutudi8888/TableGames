﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.TableGame.SimpleGames.Rules
{
  public class ShieldEffect : SimpleGameEffect, IPositiveEffect
  {


    public ShieldEffect( Element element = null )
    {
      Element = element;
      if ( Element == Element.金 )
        times = 3;

      else
        times = 1;
    }


    public SimpleGamePlayer Player { get; }

    public Element Element { get; }


    public override string Name
    {
      get { return Element?.Name + "盾牌"; }
    }

    public override string Description
    {
      get
      {
        if ( Element == Element.金 )
          return "可以抵挡三次普通攻击。";

        else if ( Element == Element.木 )
          return "可以抵挡一次普通攻击，且为您恢复生命。";

        else if ( Element == Element.水 )
          return "可以抵挡一次普通攻击，且卸除攻击者一切有益效果。并可确保在盾牌存续期间，不受诅咒影响。";

        else if ( Element == Element.火 )
          return "可以抵挡一次普通攻击，且给予攻击者伤害。";

        else if ( Element == Element.土 )
          return "可以抵挡一次普通攻击，且攻击者将无法行动一个回合。";

        else if ( Element == null )
          return "可以抵挡一次普通攻击。";

        else
          throw new InvalidOperationException( "未知的属性" );
      }
    }



    protected override async Task OnAttacked( AttackEvent attackEvent )
    {
      var recipient = attackEvent.RecipientPlayer;
      var initiate = attackEvent.InitiatePlayer;

      if ( (bool?) attackEvent.DataBag.ShieldDisabled == true )
        return;



      attackEvent.Handled = true;

      if ( attackEvent.Element == null )
      {
        attackEvent.AnnounceAttackIneffective();



        times--;
        if ( Element == Element.金 )
        {
          if ( times > 0 )
            recipient.PlayerHost.WriteWarningMessage( $"金属性盾牌阻挡了 {attackEvent.AttackPoint} 点攻击，还剩 {times} 次机会。" );

          else
            recipient.PlayerHost.WriteWarningMessage( $"金属性盾牌阻挡了 {attackEvent.AttackPoint} 点攻击，防御效果已经失效" );
        }
        else if ( Element == Element.木 )
        {
          recipient.HealthPoint += attackEvent.AttackPoint;
          recipient.PlayerHost.WriteWarningMessage( $"木属性的盾牌使得对您的攻击转变为治疗，您获得了 {attackEvent.AttackPoint} 点生命值，目前生命值为 {recipient.HealthPoint} ，防御效果已经失效" );
        }
        else if ( Element == Element.水 )
        {
          initiate.Purify( true );
          recipient.PlayerHost.WriteWarningMessage( $"水属性的盾牌阻挡了 {attackEvent.AttackPoint} 点攻击，并清除攻击方的一切效果，防御效果已经失效" );
        }
        else if ( Element == Element.火 )
        {
          recipient.PlayerHost.WriteWarningMessage( $"火属性的盾牌阻挡了 {attackEvent.AttackPoint} 点攻击，并对攻击方造成同等伤害，防御效果已经失效" );
          await recipient.Game.SendGameEvent( new AttackEvent( recipient, initiate, null, attackEvent.AttackPoint ) );
        }
        else if ( Element == Element.土 )
        {
          initiate.Confine();
          recipient.PlayerHost.WriteWarningMessage( $"土属性的盾牌阻挡了 {attackEvent.AttackPoint} 点攻击，并禁锢攻击方一回合，防御效果已经失效" );
        }

        else if ( Element == null )
          recipient.PlayerHost.WriteWarningMessage( $"您使用盾牌阻挡了 {attackEvent.AttackPoint} 点攻击，防御效果已经失效" );

        else
          throw new InvalidOperationException();

      }
      else
      {


        if ( Element == null )
        {
          recipient.PlayerHost.WriteWarningMessage( $"您的盾牌完全无法抵挡{attackEvent.Element}属性的攻击，盾牌已经被击碎" );
          attackEvent.Handled = false;
          recipient.Effects.RemoveEffect( this );
        }
        else if ( attackEvent.Element == Element )
        {
          recipient.PlayerHost.WriteWarningMessage( $"您受到{attackEvent.Element.Name}属性的攻击，与您的盾牌属性相同，盾牌完全没能发挥作用。" );
          attackEvent.Handled = false;
        }
        else if ( attackEvent.Element.IsCounteract( Element ) )
        {
          attackEvent.AnnounceDoubleAttack();
          recipient.HealthPoint -= attackEvent.AttackPoint * 2;

          recipient.PlayerHost.WriteWarningMessage( $"您受到{attackEvent.Element.Name}属性的攻击，与您的盾牌属性相克，造成了双倍伤害 {attackEvent.AttackPoint * 2} 点，目前生命值 {recipient.HealthPoint} 点。" );
        }

        else if ( attackEvent.Element.IsReinforce( Element ) )
        {
          attackEvent.AnnounceAttackIneffective();
          recipient.HealthPoint += attackEvent.AttackPoint;

          recipient.PlayerHost.WriteWarningMessage( $"您受到{attackEvent.Element.Name}属性的攻击，与您的盾牌属性相生，所受伤害变为治疗，增加生命值 {attackEvent.AttackPoint} 点，目前生命值 {recipient.HealthPoint} 点。" );
        }
        else
        {
          attackEvent.AnnounceAttackIneffective();
          recipient.PlayerHost.WriteWarningMessage( $"{Element.Name}属性的盾牌阻挡了 {attackEvent.AttackPoint} 点{attackEvent.Element}属性攻击，防御效果已经失效" );
        }

        if ( attackEvent.Handled )
          times = 0;
      }

      if ( times == 0 )
        recipient.Effects.RemoveEffect( this );

    }


    private int times;


    public override string ToString()
    {
      return (Element?.Name ?? "盾") + "D";
    }

  }
}
