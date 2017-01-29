using System;
using System.Windows.Forms;

namespace PKHack
{
    public abstract class RomObjectHandler
    {
        public abstract void ReadClass(Rom rom);
        public abstract void WriteClass(Rom rom);
    }


    /*
     * Base class for most game object classes
     */
    public abstract class RomObject
    {
        private Rom parent;
        private string id;

        protected int address;
        protected int index;

        /*
         * Properties
         */
        public Rom Parent
        {
            get { return parent; }
            set
            {
                parent = value;

                // TODO: this also needs to update the new and old
                // parents
            }
        }

        public string ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public int Address
        {
            get
            {
                return address;
            }
        }



        /*
         * Methods
         */
        public static void ReadClassFromRom(Rom rom)
        {
            throw new Exception("RomObject classes must implement a new static ReadClass method!");
        }

        public static void WriteClass(Rom rom)
        {
            throw new Exception("RomObject classes must implement a new static WriteClass method!");
        }


        // Called when this object is added to a ROM, I guess
        public void AddToRom()
        {

        }

        public void ShowType()
        {
            MessageBox.Show("I am a " + GetType().Name);
        }

        public abstract void Read(int index);
        public abstract void Write(int index);
    }
}
