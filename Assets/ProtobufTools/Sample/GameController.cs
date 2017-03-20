using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

public class GameController
{
    private static GameController m_self = null;
    public static GameController Instance
    {
        get
        {
            if (m_self == null)
            {
                m_self = new GameController();
            }
            return m_self;
        }
    }

    private GameController() { }

    public PlayerInfo player { get; set; }
}
