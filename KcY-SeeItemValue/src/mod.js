"use strict";

const liveTable = DatabaseServer.tables.templates.prices;
const itemsTable = DatabaseServer.tables.templates.handbook.Items;

class Mod
{
    name = "KcY-SeeItemValue";
    version = "1.0.0";
    static price = "";

    constructor()
    {
        Logger.info(`Loading: ${this.name} : ${this.version}`);
        ModLoader.onLoad[this.name] = this.init.bind(this);
    }

    init()
    {
        this.onLoadMod();
    }

    onLoadMod()
    {
        HttpRouter.onDynamicRoute["/cwx/itemvaluemod/"] = {
            ItemValueMod: this.onRequestConfig.bind(this)
        };
    }

    onRequestConfig(url, info, sessionID)
    {
        const splittedUrl = url.split("/");
        const id = splittedUrl[splittedUrl.length - 1].toLowerCase();

        return HttpResponse.noBody(this.getIdPrice(id));
    }

    getIdPrice(id)
    {
        let sPrice = 1;
        const result = liveTable[id];
        // Checks Flea AVG price, if null, get price from Handbook
        if(typeof result != `undefined`)
        {
            return result
        }
        for(let i in itemsTable)
        {
            if(itemsTable[i].Id === id)
            {
                return itemsTable[i].Price
            }
        }
        return sPrice;
    }
}

module.exports.Mod = Mod;