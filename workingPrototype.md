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
"5265636f72645472616e73666572416e645554584f" ["AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y","1500000000",b"211eb9cb250574f3e061e8506ebbbee91b3aebbc0a1f8018e7ebaecf907e4070"]

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
