﻿# TrelloToDevOps
Trello to Azure DevOps is a simple code that gets a trello list of cards and transposes it to Azure DevOps. After creating a bug in DEVOPs, it deletes the card from trello. 

It is currently used by me after creating a power automate flow that creates issues in trello (I do not have the licence to do the same in DEVOPs). 

You should change the tokens and the keys for both trello and Azure. 

You should first get the list Id from trello by calling the trello API with the board id. 
