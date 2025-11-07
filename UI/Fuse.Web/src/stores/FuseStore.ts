import { defineStore } from "pinia";
import { useFuseClient } from "../composables/useFuseClient";
import type { SecurityUserInfo } from "../api/client";

const fuseClient = useFuseClient

export const useFuseStore = defineStore("fuse", {
  state: () => ({
    requireSetup: false as boolean,
    currentUser: null as SecurityUserInfo | null
  }),
  actions: {
    async fetchStatus() {
      const status = await fuseClient().state();
      this.requireSetup = status.requiresSetup || false;
      this.currentUser = status.currentUser || null;

      if(this.requireSetup) {
        // navigate to the security page
        window.location.href = "/security";
      }
    }
  }
});


/*
{
  "Level": "None",
  "UpdatedAt": "2025-11-07T18:44:58.5003864Z",
  "RequiresSetup": true,
  "HasUsers": false,
  "CurrentUser": null
}
*/