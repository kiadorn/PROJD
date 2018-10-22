// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:5,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:14,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-6373-OUT,alpha-6449-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32495,y:32646,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Color,id:5270,x:32349,y:33125,ptovrint:False,ptlb:Color_copy,ptin:_Color_copy,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Time,id:8520,x:31912,y:32939,varname:node_8520,prsc:2;n:type:ShaderForge.SFN_Vector1,id:7416,x:32077,y:33128,varname:node_7416,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:8484,x:32046,y:32997,varname:node_8484,prsc:2|A-8520-T,B-7416-OUT;n:type:ShaderForge.SFN_Tex2d,id:8491,x:32111,y:32767,ptovrint:False,ptlb:node_3345,ptin:_node_3345,varname:node_3345,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:24aa3fde63499424192e80d143064b5a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Smoothstep,id:2078,x:32331,y:32784,varname:node_2078,prsc:2|A-8784-OUT,B-8784-OUT,V-8491-RGB;n:type:ShaderForge.SFN_Vector1,id:8784,x:32057,y:32687,varname:node_8784,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:6449,x:32538,y:33125,varname:node_6449,prsc:2,v1:0;n:type:ShaderForge.SFN_Fresnel,id:77,x:32334,y:32907,varname:node_77,prsc:2|EXP-5261-OUT;n:type:ShaderForge.SFN_Vector1,id:5261,x:32111,y:32922,varname:node_5261,prsc:2,v1:3.5;n:type:ShaderForge.SFN_Add,id:6373,x:32538,y:32861,varname:node_6373,prsc:2|A-2078-OUT,B-77-OUT;n:type:ShaderForge.SFN_Smoothstep,id:7828,x:32528,y:32987,varname:node_7828,prsc:2|A-7416-OUT,B-7416-OUT,V-2078-OUT;proporder:7241-8491;pass:END;sub:END;*/

Shader "Shader Forge/Yee" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _node_3345 ("node_3345", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend DstAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _node_3345; uniform float4 _node_3345_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_8784 = 0.5;
                float4 _node_3345_var = tex2D(_node_3345,TRANSFORM_TEX(i.uv0, _node_3345));
                float3 node_2078 = smoothstep( float3(node_8784,node_8784,node_8784), float3(node_8784,node_8784,node_8784), _node_3345_var.rgb );
                float node_77 = pow(1.0-max(0,dot(normalDirection, viewDirection)),3.5);
                float3 emissive = (node_2078+node_77);
                float3 finalColor = emissive;
                return fixed4(finalColor,0.0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
