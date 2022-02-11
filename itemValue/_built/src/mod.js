"use strict";

const cfg = require("./config.json");
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
        // if TraderPrice in cfg is False get price from flea AVG
        if(cfg.TraderPrice === false)
        {
            const result = liveTable[id];
            if(typeof result != `undefined`)
            {
                return result
            }
            // will still default to Handbook if no price is found for flea AVG
        }
        // if TraderPrice in cfg is True get price from handbook
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