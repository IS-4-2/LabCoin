using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace LabCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            Blockchain labcoin = new Blockchain(); // creates the blockchain and names it labcoin

            labcoin.AddBlock(new Block(1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "amount: 50")); // create a block to test it out
            labcoin.AddBlock(new Block(2, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "amount: 200")); // create a block to test it out
            
            string blockJSON = JsonConvert.SerializeObject(labcoin, Formatting.Indented); // format block in JSON
            Console.WriteLine(blockJSON); // output block

            if (labcoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is Valid!");
            }
            else
            {
                Console.Write("Blockchain is NOT valid.");
            }
        }
    }

    class Blockchain
    {
        // blockchain class properties
        public List<Block> Chain { get; set; }

        // blockchain class 
        public Blockchain()
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock()); // adds genesis block to beginning of chain
        }

        // creates first block that all other blocks chain off of
        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHmmssffff"), "GENESIS BLOCK");
        }

        public Block GetLatestBlock()
        {
            return this.Chain.Last(); // simply returns the last block on the block chain List
        }

        public void AddBlock(Block newBlock)
        {
            newBlock.PreviousHash = this.GetLatestBlock().Hash; // grabs the hash from the latest block and assigns it to new block property
            newBlock.Hash = newBlock.CalculateHash(); // after adding previous hash, calculate new hash for this new block, thus chaining it to existing chain
            this.Chain.Add(newBlock); // add newly calculated block to chain
        }

        public bool IsChainValid()
        {
            // loop through each block in the chain
            for (int i = 1; i < this.Chain.Count; i++)
            {
                // assign blocks we're looking at
                Block currentBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                // recalculate each hash to verify the calculation was done correctly
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                // check if the hash matches the previous hash
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }                
            }

            return true;
        }
    }

    class Block
    {
        // block class properties
        public int Index { get; set; }
        public string PreviousHash { get; set; }
        public string Timestamp { get; set; }
        public string Data { get; set; }
        public string Hash { get; set; }


        // constructor that initializes the block
        public Block(int index, string timestamp, string data, string previousHash = "")
        {
            this.Index = index;
            this.Timestamp = timestamp;
            this.Data = data;
            this.PreviousHash = previousHash;
            this.Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            string blockData = this.Index + this.PreviousHash + this.Timestamp + this.Data;
            byte[] blockBytes = Encoding.ASCII.GetBytes(blockData); // byte array representation of string
            byte[] hashBytes = SHA256.Create().ComputeHash(blockBytes); // compute hash
            return BitConverter.ToString(hashBytes).Replace("-", ""); // return a string version of the hashed bytes
        }

    }

}
