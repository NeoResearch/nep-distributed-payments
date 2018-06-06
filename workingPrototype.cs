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
	    private static readonly byte[] gas_asset_id = { 231, 45, 40, 105, 121, 238, 108, 177, 183, 230, 93, 253, 223, 178, 227, 132, 16, 11, 141, 20, 142, 119, 88, 222, 66, 228, 22, 139, 113, 121, 44, 96 };

        // TODO - Include GAS IS for limiting the withdraw of neo or gas

        public static object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
                return validateSendAsset(operation);
            else if (Runtime.Trigger == TriggerType.Application)
            {
                Runtime.Notify("operation:");
                Runtime.Notify(operation);
                Runtime.Notify("number of parameters:");
                Runtime.Notify(args.Length);


               if (operation == "sendAsset")
               {
                    Runtime.Notify("sendAsset: Begin to record transfer for withdraw...");
                   //if (args.Length != 3)
                    //  return false;

                   byte[] address   = (byte[])args[0];
                   BigInteger value = (BigInteger)args[1];
                   byte[] utxo      = (byte[])args[2];

                   Runtime.Notify("Parameters are:");
                   Runtime.Notify(address);
                   Runtime.Notify(value);
                   Runtime.Notify(utxo);
                   sendAsset(address,value,utxo);
                   Runtime.Notify("finished call.");
                   return true;
               }
               return false;
            }

            return false; // VerificationR || ApplicationR
        }


        public static bool validateSendAsset(string s) // unused parameter (for now)
        {
                           //TODO Include key for asset ID (neo or gas)
               byte[] utxoKey = new byte[] { 3 };
               byte[] valueKey = new byte[] { 2 };
               byte[] addresKey = new byte[] { 1 };
               byte[] addressToReward = Storage.Get( Storage.CurrentContext, addresKey );
               byte[] valueToReward = Storage.Get( Storage.CurrentContext, valueKey );
               byte[] utxoWasRegistered = Storage.Get( Storage.CurrentContext, utxoKey );
               //Runtime.Notify("addresKey is:  " + addresKey);
               //Runtime.Notify("addressToReward is:  " + addressToReward);

               Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
               TransactionOutput[] outputs = tx.GetOutputs();
               TransactionInput[] inputs = tx.GetInputs();

	       //In this first prototype, the output 0 should be the one that was registered and will withdraw
               bool outputAddressWasRegistered = (outputs[0].ScriptHash.AsBigInteger() == addressToReward.AsBigInteger());
               bool theUTXOWasRegistered = (inputs[0].PrevHash == utxoWasRegistered);
               ulong valueThatIsBeingAskedToBeWithdrew = GetContributeValue(tx, addressToReward);
               bool theRequestValueWasRegistered = (valueThatIsBeingAskedToBeWithdrew == valueToReward.AsBigInteger());

               // If outputs > 0, means that more people will receive from this withdraw. In this first version only the contract can be the second
               // Letś say that the second should always be the change, if more than 1 outcome is used
               int i =0;
               foreach (TransactionOutput output in outputs)
               {
                   if(i==1)
                   {
                       if(outputs[1].ScriptHash.AsBigInteger() != GetSmartContractScriptHash().AsBigInteger() )
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

                // this function checks if UTXO exists and contains the necessary value, otherwise, withdraw request could be registered and never be accomplished
        // TODO - Modify this function to receive a vector of UTXO hash
        private static bool CheckUTXO(byte[] utxoHash, BigInteger value)
        {
            Transaction TransactionOfDesiredUTXOClaim = Blockchain.GetTransaction(utxoHash);
            // Use GetSmartContractScriptHash() because we want to know who much arrived to this contract
            ulong utxoContributedValue = GetContributeValue(TransactionOfDesiredUTXOClaim, GetSmartContractScriptHash());
            Runtime.Notify("Inside Record: Contributed value of the requested UTXO is ");
            Runtime.Notify(utxoContributedValue);


            if(utxoContributedValue > value)
                return true;

            return false;
        }



        // ============================================================
        //                     sendAsset
        // ============================================================

        public static void sendAsset(byte[] address, BigInteger value, byte[] utxo)
        {
          //Runtime.Notify("Inside Record: Contributed value is  " + GetContributeValue((Transaction)ExecutionEngine.ScriptContainer) );
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

        // get smart contract script hash
        private static byte[] GetSmartContractScriptHash()
        {
            return ExecutionEngine.ExecutingScriptHash;
        }

        // get all you contribute neo amount
        private static ulong GetContributeValue(Transaction tx, byte[] Receiver, bool neoOrGas = true)
        {
            byte[] assetIDToGetValue = neo_asset_id;
            if(!neoOrGas)
                assetIDToGetValue = gas_asset_id;

            TransactionOutput[] outputs = tx.GetOutputs();
            ulong value = 0;
            // get the total amount of Neo
            foreach (TransactionOutput output in outputs)
            {
                if (output.ScriptHash == Receiver && output.AssetId == assetIDToGetValue)
                {
                    value += (ulong)output.Value;
                }
            }
            return value;
        }

        public static string[] supportedStandards()
        {
          return new string[]{"NEP-10","NEP-11"};
        }

    }
}
