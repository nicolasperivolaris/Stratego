using System;
using System.Collections.Generic;

namespace Stratego.Model.Pieces
{
    public class PieceFactory
    {
        private Dictionary<Type, int> PiecesQuantities = new Dictionary<Type, int>();
        public event EventHandler<Piece> AmountChanged;

        public Player Player { get; private set; }

        public PieceFactory(Player player)
        {
            Player = player;
        }

        public Piece UncountedInstanceOf(Type type)
        {
            Piece result;

            switch (type)
            {
                case Type.demineur:
                    result = new Demineur(Player);
                    break;
                case Type.espion:
                    result = new Spy(Player);
                    break;
                case Type.bomb:
                case Type.drapeau:
                    result = new Piece(type, Player);
                    break;
                default:
                    result = new Movable(type, Player);
                    break;
            }

            switch (type)
            {
                case Type.bomb:
                    result.Name = "Bombe";
                    result.MaxAmount = 6;
                    break;
                case Type.marechal:
                    result.Name = "Marechal";
                    result.MaxAmount = 1;
                    break;
                case Type.general:
                    result.Name = "Général";
                    result.MaxAmount = 1;
                    break;
                case Type.colonel:
                    result.Name = "Colonel";
                    result.MaxAmount = 2;
                    break;
                case Type.commandant:
                    result.Name = "Commandant";
                    result.MaxAmount = 3;
                    break;
                case Type.capitaine:
                    result.Name = "Capitaine";
                    result.MaxAmount = 4;
                    break;
                case Type.lieutenant:
                    result.Name = "Lieutenant";
                    result.MaxAmount = 4;
                    break;
                case Type.sergent:
                    result.Name = "Sergent";
                    result.MaxAmount = 4;
                    break;
                case Type.demineur:
                    result.Name = "Démineur";
                    result.MaxAmount = 5;
                    break;
                case Type.eclaireurs:
                    result.Name = "Eclaireur";
                    result.MaxAmount = 8;
                    break;
                case Type.espion:
                    result.Name = "Espion";
                    result.MaxAmount = 1;
                    break;
                case Type.drapeau:
                    result.Name = "Drapeau";
                    result.MaxAmount = 1;
                    break;
                default:
                    result.Name = "error";
                    break;
            }


            if (!PiecesQuantities.ContainsKey(type))
                PiecesQuantities.Add(type, 0);

            return result;
        }

        /// <summary>
        /// Return null if there is already too much instance of that Piece
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Piece or null</returns>
        public Piece CountedInstanceOf(Type type)
        {
            Piece result = UncountedInstanceOf(type);

            if (PiecesQuantities[type] < result.MaxAmount)
            {
                PiecesQuantities[type]++;
                AmountChanged?.Invoke(this, result);
                return result;
            }
            return null;
        }

        public bool PutPieceBack(Piece piece)
        {
            if (PiecesQuantities[piece.Type] > 0)
            {
                PiecesQuantities[piece.Type]--;
                AmountChanged?.Invoke(this, piece);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Piece InstanceOf(String type)
        {
            switch (type)
            {
                case "drapeau": return UncountedInstanceOf(Type.drapeau);
                case "espion": return UncountedInstanceOf(Type.espion);
                case "eclaireur": return UncountedInstanceOf(Type.eclaireurs);
                case "demineur": return UncountedInstanceOf(Type.demineur);
                case "sergent": return UncountedInstanceOf(Type.sergent);
                case "lieutenant": return UncountedInstanceOf(Type.lieutenant);
                case "capitaine": return UncountedInstanceOf(Type.capitaine);
                case "commandant": return UncountedInstanceOf(Type.commandant);
                case "colonel": return UncountedInstanceOf(Type.colonel);
                case "general": return UncountedInstanceOf(Type.general);
                case "marechal": return UncountedInstanceOf(Type.marechal);
                case "bombe": return UncountedInstanceOf(Type.bomb);
                default: return null;
            }
        }


        public Dictionary<Type, Piece> GetNewPiecesSet()
        {
            return new Dictionary<Type, Piece>
            {
                { Type.bomb, UncountedInstanceOf(Type.bomb) },
                { Type.marechal, UncountedInstanceOf(Type.marechal) },
                { Type.general, UncountedInstanceOf(Type.general) },
                { Type.colonel, UncountedInstanceOf(Type.colonel) },
                { Type.commandant, UncountedInstanceOf(Type.commandant) },
                { Type.capitaine, UncountedInstanceOf(Type.capitaine) },
                { Type.lieutenant, UncountedInstanceOf(Type.lieutenant) },
                { Type.sergent, UncountedInstanceOf(Type.sergent) },
                { Type.demineur, UncountedInstanceOf(Type.demineur) },
                { Type.eclaireurs, UncountedInstanceOf(Type.eclaireurs) },
                { Type.espion, UncountedInstanceOf(Type.espion) },
                { Type.drapeau, UncountedInstanceOf(Type.drapeau) }
            };
        }

        public int GetCount(Type type)
        {
            return PiecesQuantities[type];
        }

        public int GetCount(Piece piece)
        {
            return PiecesQuantities[piece.Type];
        }
    }
}
