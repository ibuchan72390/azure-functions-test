<template>
  <manage-person></manage-person>
</template>

<script lang="ts">
import Vue from 'vue'
import { Component } from 'nuxt-property-decorator'

@Component
export default class Create extends Vue {
  public mounted() {
    if (this.$route.params.id) {
    }
  }
}
</script>
