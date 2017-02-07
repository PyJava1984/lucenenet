using Lucene.Net.Support;
using System.Collections.Generic;
using System.Text;

namespace Lucene.Net.Search.Spans
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    using ToStringUtils = Lucene.Net.Util.ToStringUtils;

    /// <summary>
    ///   Only return those matches that have a specific payload at
    ///  the given position.
    /// <p/>
    /// Do not use this with an SpanQuery that contains a <seealso cref="Lucene.Net.Search.Spans.SpanNearQuery"/>.  Instead, use
    /// <seealso cref="SpanNearPayloadCheckQuery"/> since it properly handles the fact that payloads
    /// aren't ordered by <seealso cref="Lucene.Net.Search.Spans.SpanNearQuery"/>.
    ///
    ///
    /// </summary>
    public class SpanPayloadCheckQuery : SpanPositionCheckQuery
    {
        protected readonly ICollection<byte[]> m_payloadToMatch;

        ///
        /// <param name="match"> The underlying <seealso cref="Lucene.Net.Search.Spans.SpanQuery"/> to check </param>
        /// <param name="payloadToMatch"> The <seealso cref="java.util.Collection"/> of payloads to match </param>
        public SpanPayloadCheckQuery(SpanQuery match, ICollection<byte[]> payloadToMatch)
            : base(match)
        {
            if (match is SpanNearQuery)
            {
                throw new System.ArgumentException("SpanNearQuery not allowed");
            }
            this.m_payloadToMatch = payloadToMatch;
        }

        protected override AcceptStatus AcceptPosition(Spans spans)
        {
            bool result = spans.IsPayloadAvailable;
            if (result == true)
            {
                var candidate = spans.GetPayload();
                if (candidate.Count == m_payloadToMatch.Count)
                {
                    //TODO: check the byte arrays are the same
                    var toMatchIter = m_payloadToMatch.GetEnumerator();
                    //check each of the byte arrays, in order
                    //hmm, can't rely on order here
                    foreach (var candBytes in candidate)
                    {
                        toMatchIter.MoveNext();
                        //if one is a mismatch, then return false
                        if (Arrays.Equals(candBytes, toMatchIter.Current) == false)
                        {
                            return AcceptStatus.NO;
                        }
                    }
                    //we've verified all the bytes
                    return AcceptStatus.YES;
                }
                else
                {
                    return AcceptStatus.NO;
                }
            }
            return AcceptStatus.YES;
        }

        public override string ToString(string field)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("spanPayCheck(");
            buffer.Append(m_match.ToString(field));
            buffer.Append(", payloadRef: ");
            foreach (var bytes in m_payloadToMatch)
            {
                ToStringUtils.ByteArray(buffer, bytes);
                buffer.Append(';');
            }
            buffer.Append(")");
            buffer.Append(ToStringUtils.Boost(Boost));
            return buffer.ToString();
        }

        public override object Clone()
        {
            SpanPayloadCheckQuery result = new SpanPayloadCheckQuery((SpanQuery)m_match.Clone(), m_payloadToMatch);
            result.Boost = Boost;
            return result;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is SpanPayloadCheckQuery))
            {
                return false;
            }

            SpanPayloadCheckQuery other = (SpanPayloadCheckQuery)o;
            return this.m_payloadToMatch.Equals(other.m_payloadToMatch) && this.m_match.Equals(other.m_match) && this.Boost == other.Boost;
        }

        public override int GetHashCode()
        {
            int h = m_match.GetHashCode();
            h ^= (h << 8) | ((int)((uint)h >> 25)); // reversible
            //TODO: is this right?
            h ^= m_payloadToMatch.GetHashCode();
            h ^= Number.SingleToInt32Bits(Boost); // LUCENENET TODO: This was FloatToRawIntBits in the original
            return h;
        }
    }
}