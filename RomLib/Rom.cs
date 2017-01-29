using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;


namespace PKHack
{
	public enum Storage
	{
		Unrestricted,
		Fixed,
		Local
	}


	/// <summary>
	/// Represents a chunk of the ROM's data requested by an object for reading or
	/// writing.
	/// 
	/// A requested block should always correspond exactly to an area of strictly
	/// contiguous data within an object.
	/// </summary>
	public class Block 
	{
		// Internal data:
		//  Reference to ROM
		//  Readable/Writable specifiers
		//  Location of data within ROM
		//  Size of data (if applicable)
		//private Rom rom;
		private byte[] data;
		//private byte[] buffer;	// for write operations
		private int address;
		private int pointer;
		private int size;
		private bool writable;


		public Block(byte[] data, int location, bool writable)
		{
			this.data = data;
			this.size = -1;
			this.address = location;
			this.pointer = location;
			this.writable = writable;
		}

		public void Write(int value)
		{
			if (pointer + sizeof(int) >= address + size)
				throw new Exception("Block write overflow!");
			data[pointer++] = (byte)value;
			data[pointer++] = (byte)(value >> 8);
			data[pointer++] = (byte)(value >> 16);
			data[pointer++] = (byte)(value >> 24);
		}

		public void Write(short value)
		{
			if (pointer + sizeof(short) >= address + size)
				throw new Exception("Block write overflow!");
			data[pointer++] = (byte)value;
			data[pointer++] = (byte)(value >> 8);
		}

		public void Write(byte value)
		{
			if (pointer + sizeof(byte) >= address + size)
				throw new Exception("Block write overflow!");
			data[pointer++] = value;
		}


		/// <summary>
		/// Reads a value and increments the block's current position.
		/// </summary>
		/// <param name="value">An 'out' variable into which to read the data</param>
		public void Read(out int value)
		{
			value = (int)(data[pointer++]
				+ (data[pointer++] << 8)
				+ (data[pointer++] << 16)
				+ (data[pointer++] << 24));
		}

		public void Read(out short value)
		{
			value = (short)(data[pointer++]
				+ (data[pointer++] << 8));
		}

		public void Read(out char value)
		{
			value = (char)data[pointer++];
		}

		public void Read(out byte value)
		{
			value = data[pointer++];
		}

		/// <summary>
		/// Reads a 32-bit integer from the block's current position and
		/// advances the current position by 4 bytes.
		/// </summary>
		/// <returns>The 32-bit value at the current position.</returns>
		public int ReadInt()
		{
			int value;
			Read(out value);
			return value;
		}

		/// <summary>
		/// Reads a 16-bit integer from the block's current position and
		/// advances the current position by 2 bytes.
		/// </summary>
		/// <returns>The 16-bit value at the current position.</returns>
		public short ReadShort()
		{
			short value;
			Read(out value);
			return value;
		}

		/// <summary>
		/// Reads a single byte from the block's current position and
		/// increments the current position.
		/// </summary>
		/// <returns>The byte at the current position.</returns>
		public byte ReadByte()
		{
			byte value;
			Read(out value);
			return value;
		}


		/// <summary>
		/// Decompresses data from the block's current position. Note that
		/// this method first measures the compressed data's size before allocating
		/// the destination array, which incurs a slight additional overhead.
		/// </summary>
		/// <returns>An array containing the decompressed data.</returns>
		public byte[] Decomp()
		{
			int size = Rom.GetCompressedSize(pointer, data);
			if (size < 1)
				throw new Exception("Invalid compressed data.");

			byte[] output = new byte[size];
			int read;
			int actualSize = Rom.Decomp(pointer, data, output, out read);

			if (size != actualSize)
				throw new Exception("ERROR! Computed and actual decompressed sizes do not math. Please reinstall universe and reboot.");

			return output;
		}
	}


	/// <summary>
	/// Represents a "reservation" of a large chunk of space in the ROM which
	/// can be used for multiple objects.
	/// 
	/// This is used primarily for classes requiring the "Local" storage model,
	/// i.e., those which must have all their members stored in the same ROM bank.
	/// </summary>
	public class LocalTicket 
	{
		
	}


	/// <summary>
	/// Used to maintain a registry of available ROM object types.
	/// 
	/// This registry is static, but the class is declared non-static mainly
	/// so you can instantiate it and use its indexer. :P
	/// </summary>
	public class RomClasses 
	{
		/// <summary>
		/// Represents a registered class entry within the ROM classes registry.
		/// </summary>
		public struct Entry 
		{
			public string ID;
			public Type Type;
			public Type Handler;

			public Entry(string id, Type type, Type handler) 
			{
				ID = id;
				Type = type;
				Handler = handler;
			}
		}


		private static Dictionary<String, Entry> types = new Dictionary<String, Entry>();


		/// <summary>
		/// Gets a collection of entries for all registered classes.
		/// </summary>
		public static IEnumerable<Entry> Types
		{
			get { return types.Values; }
		}


		/// <summary>
		/// Registers a class of objects. A class must be registered before it
		/// can be used with (or by) PKHack's Rom class.
		/// </summary>
		/// <param name="id">A string that identifies this type of object. (Example: "EnemyGroup")</param>
		/// <param name="type">The type of the class representing this object.</param>
		/// <param name="handler">A RomObjectHandler-derived object that will handle loading and storing elements of the class being registered.</param>
		public static void RegisterClass(String id, Type type, Type handlerType)
		{
			// Check for collisions
			foreach (Entry e in types.Values)
			{
				if(e.ID == id)
					throw new Exception("Type ID '" + id + "' is already registered.");
				if(e.Type == type)
					throw new Exception("Type '" + type.Name + "' is already registered.");
				if (handlerType != null && e.Handler == handlerType)
					throw new Exception("Handler Type '" + handlerType.Name + "' is already registered."); 
			}

			// Verify that the type and handler type are correct
			if (!type.IsSubclassOf(typeof(RomObject)))
				throw new Exception("Type '" + type.Name + "' is not a subclass of RomObject.");
			if (handlerType != null && !handlerType.IsSubclassOf(typeof(RomObjectHandler)))
				throw new Exception("Type '" + handlerType.Name + "' is not a subclass of RomObjectHandler.");

			// If all goes well, register the ID, type, and handler
			types.Add(id, new Entry(id, type, handlerType));
		}


		/// <summary>
		/// Gets a particular registered class entry by identifier.
		/// </summary>
		/// <param name="id">The identifier of the class to retrieve.</param>
		/// <returns>The class entry registered with the specified ID.</returns>
		public Entry this[String id]
		{
			get
			{
				return types[id];
			}
		}
	}

	
	/**
	 * The ROM class. Starting point for general resource containment.
	 */
	public partial class Rom
	{
		/*
		 * INTERNAL DATA
		 */
		private string filename;
		//private FileStream stream;

		public byte[] data;
		private bool loaded;

		private Dictionary<Type, List<RomObject>> objects;
		private Dictionary<Type, RomObjectHandler> handlers;

		private RomClasses types = new RomClasses();


		/*
		 * Properties
		 */
		public Boolean IsLoaded 
		{
			get { return loaded; }
		}
		public String Filename 
		{
			get { return filename; }
		}

		public String MD5Hash
		{
			get
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				byte[] hashBytes = md5.ComputeHash(data);
				return BitConverter.ToString(hashBytes);
			}
		}


		/*
		 * Constructors
		 */
		public Rom()
		{
			objects = new Dictionary<Type, List<RomObject>>();
			loaded = false;

			// New step: every ROM needs to have its own instance of
			// each type handler.
			handlers = new Dictionary<Type, RomObjectHandler>();
			foreach (RomClasses.Entry e in RomClasses.Types)
			{
				if(e.Handler != null)
					handlers.Add(e.Type, (RomObjectHandler)Activator.CreateInstance(e.Handler));
			}
		}


		/*
		 * Static methods
		 */
		public static void RegisterType(String typeID, Type type, Type handler)
		{
			RomClasses.RegisterClass(typeID, type, handler);
		}


		/*
		 * Methods
		 */
		public void Open(Stream stream)
		{
			data = new byte[stream.Length];
			stream.Read(data, 0, (int)stream.Length);
			loaded = true;

			foreach (KeyValuePair<Type, RomObjectHandler> kvp in handlers)
			{
				//MessageBox.Show("Loading " + kvp.Key.Name);
				kvp.Value.ReadClass(this);
			}

		}
		public void Open(string filename)
		{
			try
			{
				this.filename = filename;
				FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);

				Open(stream);

				//data = new byte[stream.Length];
				//stream.Read(data, 0, (int)stream.Length);

				//loaded = true;
			}
			catch (Exception e)
			{
				// Error opening file
				MessageBox.Show(e.Message);
				data = null;
				loaded = false;

				return;
			}

			// Initialize and load all registered types
			//foreach (RomClasses.Entry ClassEntry in RomClasses.Types)
			//{
			//	ClassEntry.Handler.ReadClass(this);
			//}

		}


		public void Save()
		{
		}


		/// <summary>
		/// Adds an object to the ROM container.
		/// </summary>
		/// <param name="o">The RomObject to add</param>
		public void Add(RomObject o)
		{
			Type type = o.GetType();

			// Create a new type list if necessary
			if (!objects.ContainsKey(type))
				objects.Add(type, new List<RomObject>());

			objects[type].Add(o);

			o.Parent = this;

			// Hrm, now we need to update the damn thing's internal count...
			o.AddToRom();
		}

		public RomObject GetObject(Type type, int index)
		{
			try
			{
				return objects[type][index];
			}
			catch (Exception)
			{
				return null;
			}
		}

		public RomObject GetObject(String typename, int index)
		{
			try
			{
				return objects[types[typename].Type][index];
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a collection of all RomObjects of a given type contained
		/// within this ROM.
		/// </summary>
		/// <param name="type">The type of RomObjects to retrieve</param>
		/// <param name="typeID">The string identifiying the type of RomObjects to retrieve</param>
		/// <returns>A List of RomObjects</returns>
		public List<RomObject> GetObjectsByType(Type type)
		{
			return objects[type];
		}

		public List<RomObject> GetObjectsByType(String typeID)
		{
			return objects[types[typeID].Type];
		}


		public RomObjectHandler GetObjectHandler(Type type)
		{
			return handlers[type];
		}


		/// <summary>
		/// Returns a readable block at the given location.
		/// Nominally, should also handle tracking free space depending on
		/// the type of read requested. (i.e., an object may be interested in
		/// read-only access anywhere, but if an object is reading its own data,
		/// it should specify this so the Rom can mark the read data as "free")
		/// </summary>
		/// <param name="location">The address from which to read</param>
		/// <returns>A readable block</returns>
		public Block ReadBlock(int location) 
		{
			// NOTE: there's no address conversion implemented yet;
			// we're assuming all addresses are file offsets (with header)

			// For now, just return a readable block; we'll worry about
			// typing and free space later
			return new Block(data, location, false);

		}

		/// <summary>
		/// Allocates a writeable block using the Unrestricted storage model.
		/// The resulting block may be located anywhere in the ROM.
		/// </summary>
		/// <param name="size">The size, in bytes, required for this block</param>
		/// <returns>A writeable block, or null if allocation failed</returns>
		public Block AllocateBlock(int size) 
		{

			return null;
		}

		/// <summary>
		/// Allocates a writeable block using the Fixed storage model. The resulting
		/// block is always located at the given address.
		/// </summary>
		/// <param name="size">The size, in bytes, required for this block</param>
		/// <param name="location">The starting address of the desired block</param>
		/// <returns>A writeable block of size bytes in the specified location, or null if allocation failed</returns>
		public Block AllocateFixedBlock(int size, int location) 
		{

			return null;
		}

		/// <summary>
		/// Allocates a writeable block using the Local storage model. Reserves a block
		/// of space within a previously allocated local segment.
		/// </summary>
		/// <param name="size">The size, in bytes, required for this block</param>
		/// <param name="ticket">A local segment identifier previously obtained from
		///		AllocateLocalSegment, identifying a pre-allocated space that has been
		///		reserved for a particular set of local-access objects</param>
		/// <returns>A writeable block of size bytes in the given local segment.</returns>
		public Block AllocateLocalBlock(int size, LocalTicket ticket) 
		{

			return null;
		}

	}
}