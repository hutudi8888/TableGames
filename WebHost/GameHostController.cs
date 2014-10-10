﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Ivony.TableGame.WebHost
{
  public class GameHostController : ApiController
  {




    [HttpGet]
    public object Game( string name )
    {

      var game = Games.GetOrCreateGame( name );

      game.JoinGame( PlayerHost );
      return "OK";

    }



    [HttpGet]
    public object Status( HttpRequestMessage request, string messageMode = null )
    {
      if ( messageMode == "all" )
        PlayerHost.SetMessageIndex( 0 );



      var player = PlayerHost.Player;

      return new
      {
        GameInformation = player == null ? null : player.GetGameInformation(),
        Messages = PlayerHost.GetMessages(),
      };
    }



    public PlayerHost PlayerHost
    {
      get
      {
        return (PlayerHost) ControllerContext.Request.Properties[PlayerHostHttpHandler.playerKey];
      }
    }

  }
}