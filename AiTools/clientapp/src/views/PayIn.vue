<template>
<v-container grid-list-xl>
    <h2>Способы оплаты</h2>
    <v-layout>
        <v-flex xs12 sm9>
            <v-layout>
                <v-flex v-for="type in payTypes" :key="type.type" xs12 sm3 d-flex>
                    <v-tooltip bottom>
                        <v-card slot="activator" hover :style="{padding: type.padding ? type.padding : 0}">
                            <v-img :src="type.image" />
                        </v-card>
                        <span>{{type.title}}</span>
                    </v-tooltip>
                </v-flex>
            </v-layout>
        </v-flex>
    </v-layout>
</v-container>
</template>

<script>
import yaLogo from '../assets/images/yaMoneyIcon.png';
import qiwiLogo from '../assets/images/qiwiIcon.png';
import wmLogo from '../assets/images/wmlogo_flat_128.png';
import bankLogo from '../assets/images/bank-logo.png';

export default {
    name: 'pay-in',
    data(){
        return{
            payTypes: [
                {
                    title: 'Яндекс.Деньги',
                    type: 'yandex',
                    image: yaLogo,
                },
                {
                    title: 'QIWI',
                    type: 'qiwi',
                    image: qiwiLogo,
                },
                {
                    title: 'WebMoney',
                    type: 'wm',
                    image: wmLogo,
                    padding: '10px'
                },
                {
                    title: 'Банковская карта',
                    type: 'bank',
                    image: bankLogo,
                    padding: '10px'
                }
            ]
        };
    }
}
</script>
<style>

</style>


