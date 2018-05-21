# nep-distributed-payments
## Guidelines for a simple working prototype

Congratulations to all of us!
Good key contributions were made necessary for the comprehensions stated here.

In particular, thanks to the insights of NeoResearch and NEL team:
(https://medium.com/neweconolab/neo-smart-contract-development-ii-an-additional-exploration-of-mission-impossible-119b49666b1d)[Mission Impossible I],
(https://medium.com/neweconolab/neo-smart-contract-development-ii-an-additional-exploration-of-mission-impossible-119b49666b1d)[Mission Impossible II] and the upcomming (https://xxx)[Mission Impossible III]


## Simple steps to be followed on the  (https://neocompiler.io)[NeoCompiler Eco]:
------------------------------------------------------------------------------------------------
### Step 1 - Deploy the contract, we need storage

### Step 2 - Make a transfer to the address of the deployed contract

### Step 3 - Call "method" ["address","values(not used yet)+00000000",b"utxo" (reverted)]
EXAMPLES IN PYTHON:
"5265636f72645472616e73666572416e645554584f" ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","200000000",b"ee4a22d100d3122d17e390f26a8f48d1e8f978a05a7d1ab97701191f6ecd4039"]

This step register that AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y will receive 1 NEO OR GAS from UTXO 0056facaee834ad514b60d06c1dd43f8741dd9c4c245b22b8a1d75a11c7228df
TODO: Limit the Neo or Gas by asset id registered.
OPTION 1: Allow multiple UTXO, it is usefull for huge amount
OPTION 2: Create a function that merge UTXO, then, the last option will not be needed


### Step 4 - Call verification/withdraw transaction

Verification: 0000

Outputs: (first line should be the address of withdraw)
e9eed8dc39332032dc22e5d6e86332c50327ba23
Scripthash of the contranct

Values:
Ammount to withdraw
Amount of change to the contract


### Illustrative examples

#### Deploy
Copy the code at `simplePrototype.cs`

<p align="center"> <img
  src="/1-compile.png"
  width="500px;"></p>

#### Copy the AVM

Copy the compiled AVM opcodes.
Their are going to be used in the verification/withdrawn transaction invoking process.

<p align="center"> <img
  src="/2-copyTheAVM.png"
  width="500px;"></p>

#### Paste the AVM on the EcoLab Transaction Build
The enviroment contain the tools required for the automatic transfer

<p align="center"> <img
  src="/3-ecoLabTransactionBuild.png"
  width="500px;"></p>

Paste the AVM and copy the Address for making a transfer

<p align="center"> <img
  src="/4-pasteTheAVM.png"
  width="500px;"></p>

#### Transfer some funds from one of the NeoCompiler wallet

This step could be, for example, the MintToken of an ICO.
The ICO template would register the amount contributed.

<p align="center"> <img
  src="/5-transferSomeFunds.png"
  width="500px;"></p>

#### Selecting an UTXO: This steps will be automatic from the light wallets
We need to select a desired INPUT UTXO for registering it to be withdrew.

<p align="center"> <img
  src="/6-selectAUTXOAndFill0000.png"
  width="500px;"></p>

#### Revert the TX ID
As standard, the hex should be reverted

<p align="center"> <img
  src="/7-reverTXID.png"
  width="500px;"></p>

#### Invoke using application Trigger
This step should the check the ammount that the invoking address will/can withdraw. It also register the input UTXO given as parameter (only if it contains the necessary ammount of funds)

<p align="center"> <img
  src="/8-invoke.png"
  width="500px;"></p>

#### Not registered withdraws will return FALSE

<p align="center"> <img
  src="/9-tryToWithDrawSomethingWrong.png"
  width="500px;"></p>

<p align="center"> <img
  src="/9-trySomethingWrongII.png"
  width="500px;"></p>

#### Proceed with the automatic reward/withdraw

<p align="center"> <img
  src="/10-getTheAutomaticReward.png"
  width="500px;"></p>
