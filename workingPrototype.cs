Congratulations to all of us! 
Good compreenshion.


------------------------------------------------------------------------------------------------
Step 1 - Deploy the contract, we need storage

Step 2 - Make a transfer to the address of the deployed contract

Step 3 - Call "method" ["address","values(not used yet)+00000000",b"utxo" (reverted)] 
EXAMPLES IN PYTHON:
"5265636f72645472616e73666572416e645554584f" ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","1500000000",b"45a6d2fa9d171ad1c9b9f71d15499231dc4a8d54be3ce42f17ed5bc4a7e85e26"]

This step register that AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y will receive 1 NEO OR GAS from UTXO 0056facaee834ad514b60d06c1dd43f8741dd9c4c245b22b8a1d75a11c7228df
TODO: Limit the Neo or Gas by asset id registered.
OPTION 1: Allow multiple UTXO, it is usefull for huge amount
OPTION 2: Create a function that merge UTXO, then, the last option will not be needed


Step 4 - Call verification/withdraw transaction

Verification: 0000

Outputs: (first line should be the address of withdraw)
e9eed8dc39332032dc22e5d6e86332c50327ba23
Scripthash of the contranct

Values:
Ammount to withdraw
Amount of change to the contract

------------------------------------------------------------------------------------------------ 


using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

// This example is intended to be the simplest example of NEP10 proposal
// Since many things can occur in network for more complex scenarios, see other related examples
namespace NeoContract1
{
    public class NEP10SimpleExample : SmartContract
    {

        private static readonly byte[] neo_asset_id = { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147, 229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 };
        // TODO - Include GAS IS for limiting the withdraw of neo or gas
      
        public static object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
               byte[] utxoKey = new byte[] { 3 }; // first status -> Waiting editor acceptance
               byte[] valueKey = new byte[] { 2 }; // first status -> Waiting editor acceptance
               byte[] addresKey = new byte[] { 1 }; // first status -> Waiting editor acceptance
               byte[] addressToReward = Storage.Get( Storage.CurrentContext, addresKey );
               byte[] valueToReward = Storage.Get( Storage.CurrentContext, valueKey );
               byte[] utxoWasRegistered = Storage.Get( Storage.CurrentContext, utxoKey );
               //Runtime.Notify("addresKey is:  " + addresKey);
               //Runtime.Notify("addressToReward is:  " + addressToReward);
               //Runtime.Notify("sender is:  " + GetSender());
               
               Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
               TransactionOutput[] outputs = tx.GetOutputs();
               TransactionInput[] inputs = tx.GetInputs();
               
               bool outputAddressWasRegistered = (outputs[0].ScriptHash.AsBigInteger() == addressToReward.AsBigInteger());
               bool theUTXOWasRegistered = (inputs[0].PrevHash == utxoWasRegistered);

               ulong valueThatIsBeingAskedToBeWithdrew = GetContributeValue(tx, addressToReward);
               bool theRequestValueWasRegistered = (valueThatIsBeingAskedToBeWithdrew == valueToReward.AsBigInteger());
       
               //TODO - An additional check is still required
               // If outputs > 0, means that more people will receive from this withdraw. In this first version only the contract can be the second
               // Letś say that the second should always be the change, if more than 1 outcome is used
               int i =0;
               foreach (TransactionOutput output in outputs)
               {
                   if(i==1)
                   {
                       if(outputs[1].ScriptHash.AsBigInteger() != GetReceiver().AsBigInteger()  )
                           return false;
                   }
                   
                   if(i>2)
                    return false;
                   i++;
               }       
               
               
               //bool checkAssetID = (outputs[0].AssetId == neo_asset_id) if variable saved in storage is 0 or gas if 1
               if(outputAddressWasRegistered && theUTXOWasRegistered && theRequestValueWasRegistered)
               {
                   return true;
               }
          
              
               return false;
            }
            else // Application
            {
              Runtime.Notify("operation:");
              Runtime.Notify(operation);
              Runtime.Notify("number of parameters:");
              Runtime.Notify(args.Length);

 
               if (operation == "RecordTransferAndUTXO")
               {
                    Runtime.Notify("Begin to record transfer for withdraw...");
                   //if (args.Length != 3)
                    //  return false;
                      
                   byte[] address =(byte[])args[0];
                   BigInteger value = (BigInteger)args[1];
                   byte[] utxo = (byte[])args[2];
                    
                   Runtime.Notify("Parameters are:");    
                   Runtime.Notify(address);
                   Runtime.Notify(value);
                   Runtime.Notify(utxo);
                   RecordTransferAndUTXO(address,value,utxo);
                   Runtime.Notify("finished call.");
                   return true;
               }
               return false;
            }
            //unreachable (or return false)
        }

        // this function checks if UTXO exists and contains the necessary value, otherwise, withdraw request could be registered and never be accomplished
        // TODO - Modify this function to receive a vector of UTXO hash
        public static bool CheckUTXO(byte[] utxoHash, BigInteger value)
        {
            Transaction TransactionOfDesiredUTXOClaim = Blockchain.GetTransaction(utxoHash);
            // Use GetReceiver() because we want to know who much arrived to this contract
            ulong utxoContributedValue = GetContributeValue(TransactionOfDesiredUTXOClaim, GetReceiver());
            Runtime.Notify("Inside Record: Contributed value of the requested UTXO is ");
            Runtime.Notify(utxoContributedValue);
            

            if(utxoContributedValue > value)
                return true;
            
            return false;
        }    
        // auxiliar method for printing (not part of NEP10)
        public static void RecordTransferAndUTXO(byte[] address, BigInteger value, byte[] utxo)
        {
          //Runtime.Notify("Inside Record: Contributed value is  " + GetContributeValue((Transaction)ExecutionEngine.ScriptContainer) );
          //Runtime.Notify("Inside Record: Sender is  " + GetSender() );
          if(!CheckUTXO(utxo,value))
          {
              Runtime.Notify("Be carefull, you should pass a valid UTXO.");    
              return;
          }
          
          byte[] utxoKey = new byte[] { 3 }; // first status -> Waiting editor acceptance
          byte[] valueKey = new byte[] { 2 }; // first status -> Waiting editor acceptance
          byte[] addresKey = new byte[] { 1 }; // first status -> Waiting editor acceptance
          Storage.Put( Storage.CurrentContext, addresKey, address); //writing the data
          Storage.Put( Storage.CurrentContext, valueKey, value ); //writing the data //always 1 - it means that the guy will only be able to withdraw one
          Storage.Put( Storage.CurrentContext, utxoKey, utxo ); //writing the data
          
          Runtime.Notify("What is stored is:");    
          Runtime.Notify(Storage.Get( Storage.CurrentContext, addresKey ));
          Runtime.Notify(Storage.Get( Storage.CurrentContext, valueKey ));
          Runtime.Notify(Storage.Get( Storage.CurrentContext, utxoKey ));
        }
      

        private static byte[] GetSender()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput[] reference = tx.GetReferences();

            // you can choice refund or not refund
            foreach (TransactionOutput output in reference)
            {
                Runtime.Notify("Outputs");
                Runtime.Notify(output.ScriptHash);
                Runtime.Notify(output.ScriptHash.AsBigInteger());
                if (output.AssetId == neo_asset_id) return output.ScriptHash;
            }
            return new byte[]{};
        }
        
        // get smart contract script hash
        private static byte[] GetReceiver()
        {
            return ExecutionEngine.ExecutingScriptHash;
        }
        
        // get all you contribute neo amount
        private static ulong GetContributeValue(Transaction tx, byte[] Receiver, bool neoOrGas = true)
        {
            byte[] assetToGetValue = neo_asset_id;
            if(!neoOrGas)
               assetToGetValue = neo_asset_id; // TODO - Change to GAS
               
            TransactionOutput[] outputs = tx.GetOutputs();
            ulong value = 0;
            // get the total amount of Neo
            // 获取转入智能合约地址的Neo总量
            foreach (TransactionOutput output in outputs)
            {
                if (output.ScriptHash == Receiver && output.AssetId == neo_asset_id)
                {
                    value += (ulong)output.Value;
                }
            }
            return value;
        }

    }
}
